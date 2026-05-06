using HelpDeskTicketing.Core.DTOs;
using HelpDeskTicketing.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskTicketing.Controllers;
[ApiController]
[Route("api/auth")]
public class AuthController:ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    [HttpPost("register")]
    public async Task<ActionResult<string>> RegisterAsync([FromBody] RegisterUserDTO registerUserDto)
    {
        return Ok(await _authService.RegisterAsync(registerUserDto));
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> LoginAsync([FromQuery] string email,[FromQuery] string password,
        CancellationToken cancelationToken = default)
    {
        return Ok(await _authService.LoginAsync(email, password, cancelationToken));
    }

}