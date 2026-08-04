namespace PQMS.API.DTOs.Patient;

public record PatientCreateDto(
    string FullName,
    string? PhoneNumber,
    DateTime? DateOfBirth
);
