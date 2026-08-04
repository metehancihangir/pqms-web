using PQMS.API.DTOs.Auth;
using PQMS.API.Models;

namespace PQMS.API.Services;

public interface IAuthService
{
    Task<AuthResponseDto> Login(LoginRequestDto request);
    Task<AuthResponseDto> Register(RegisterRequestDto request);
    string GenerateJwtToken(User user);
}
