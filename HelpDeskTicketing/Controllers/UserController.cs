using HelpDeskTicketing.Core.DTOs;
using HelpDeskTicketing.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskTicketing.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UserController:ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsersAsync([FromQuery]int page,[FromQuery] int pageSize,
        CancellationToken cancellationToken = default)
    {
        var users =  await _userService.GetAllUsersAsync(page, pageSize, cancellationToken);
        if (users == null || !users.Any())
        {
            return NotFound("No users found.");
        }
        return Ok(users);
    }

    [HttpDelete("{userId}")]
    public async Task<ActionResult> DeleteUserByIdAsync([FromRoute]int userId, CancellationToken cancellationToken = default)
    {
        await _userService.DeleteUserByIdAsync(userId, cancellationToken);
        return NoContent();
    }
}