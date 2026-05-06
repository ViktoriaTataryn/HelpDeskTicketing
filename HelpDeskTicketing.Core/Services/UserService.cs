using HelpDeskTicketing.Core.DTOs;
using HelpDeskTicketing.Core.Interfaces;
using HelpDeskTicketing.Entities.Models;
using HelpDeskTicketing.Storage;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskTicketing.Core.Services;

public class UserService: IUserService //only admin
{
    private readonly HelpDeskContext _context;

    public UserService(HelpDeskContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Select(x => new UserDto
            {
                Id = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
            })
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
        return user;

    }

    public async Task DeleteUserByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FindAsync([userId], cancellationToken);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            throw new ArgumentException("User not found");
        }

    }

    public bool IsAdmin(int userId)
    {
       var user = _context.Users
           .Where(x => x.Id == userId)
           .Select(x=>x.Role)
           .FirstOrDefault();
       return user == Role.Admin;

    }
}