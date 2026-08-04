namespace PQMS.API.DTOs.Patient;

public record PatientSearchResultDto(
    int Id,
    string FullName,
    string? PhoneNumber,
    DateTime? DateOfBirth
);
