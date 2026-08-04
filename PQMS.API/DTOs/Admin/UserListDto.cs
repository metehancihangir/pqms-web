namespace PQMS.API.DTOs.Admin;

public record UserListDto(
    int Id, 
    string FullName, 
    string Email, 
    string Role, 
    bool IsActive, 
    DateTime CreatedAt
);
