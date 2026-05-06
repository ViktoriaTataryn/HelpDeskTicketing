using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using HelpDeskTicketing.Core.DTOs;
using HelpDeskTicketing.Core.Interfaces;
using HelpDeskTicketing.Entities.Models;
using HelpDeskTicketing.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HelpDeskTicketing.Core.Services;

public class AuthService : IAuthService
{
    private readonly HelpDeskContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthService(HelpDeskContext context, IMapper mapper, IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<string> RegisterAsync(RegisterUserDTO registerUserDTO, CancellationToken cancelationToken = default)
    {
        /*//validation 
        if (!Regex.IsMatch(registerUserDTO.Email, @"^[a-zA-Z0-9_\-\.]+$"))
        {
            throw new ArgumentException("Email is invalid", nameof(registerUserDTO.Email));
        }

        if (!Regex.IsMatch(registerUserDTO.PasswordHash, @"^.*(?=.{8,})(?=.*[a-zA-Z])(?=.*\d)(?=.*[!#$%&? ""]).*$"))
        {
            throw new ArgumentException("Password is invalid", nameof(registerUserDTO.PasswordHash));
        }*/

        if (await _context.Users.AnyAsync(u => u.Email.Equals(registerUserDTO.Email), cancelationToken))
        {
            throw new ArgumentException("Email is already taken", nameof(registerUserDTO.Email));
        }
        var user = _mapper.Map<User>(registerUserDTO);
        user.Email = user.Email.ToLower();
        user.CreatedAt=DateTime.UtcNow;
        user.Role = Role.User;
        user.PasswordHash=PasswordHasher.GenerateHash(registerUserDTO.PasswordHash,user.CreatedAt);
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancelationToken);
        return GenerateToken(user);
    }

    public async Task<string> LoginAsync(string email, string password, CancellationToken cancelationToken = default)
    {
        var user =await _context.Users.SingleOrDefaultAsync(u => u.Email.Equals(email));
        if (user == null)
        {
            throw new ArgumentException("User not found", nameof(email));
        }
        var uPassw =  PasswordHasher.GenerateHash(password, user.CreatedAt);
        if (user.PasswordHash != uPassw)
        {
            throw new ArgumentException("Wrong password", nameof(user.PasswordHash));
        }
        
        return GenerateToken(user);
    }
    

    private string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:Token"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;  
    }

}

