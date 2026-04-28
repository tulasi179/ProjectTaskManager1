using Projecttaskmanager.Models;
using Projecttaskmanager.DTOs;

namespace Projecttaskmanager.Services;

public interface IAuthService
{
     Task<(Users? User, string? Error)> RegisterAsync(UserResponce request);
    //Task<TokenResponce?> LoginAsync(UserResponce request);
    Task<(TokenResponce? Token, string? Error)> LoginAsync(UserResponce request);
    Task<TokenResponce?> RefreshTokensAsync(RefreshTokenRequestDto request);
      Task ResetPasswordAsync(ResetPasswordDto dto);
}