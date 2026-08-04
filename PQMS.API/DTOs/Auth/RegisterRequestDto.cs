namespace PQMS.API.DTOs.Auth;

public record RegisterRequestDto(string FullName, string Email, string Password, string Role);
