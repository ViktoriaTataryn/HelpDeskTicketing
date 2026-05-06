using HelpDeskTicketing.Core.DTOs;

namespace HelpDeskTicketing.Core.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterUserDTO registerUserDTO, CancellationToken cancelationToken = default);
    Task<string> LoginAsync(string email, string password, CancellationToken cancelationToken = default);
}