using HelpDeskTicketing.Core.DTOs;

namespace HelpDeskTicketing.Core.Interfaces;

public interface IUserService 
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync(int page, int pageSize,CancellationToken cancellationToken=default);
    Task DeleteUserByIdAsync(int userId, CancellationToken cancellationToken=default);
    bool IsAdmin(int userId);
}