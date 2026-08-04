namespace PQMS.API.DTOs.Admin;

public record UpdateUserRoleDto(
    string Role, 
    bool IsActive
);
