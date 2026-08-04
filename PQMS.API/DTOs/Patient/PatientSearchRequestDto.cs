namespace PQMS.API.DTOs.Patient;

public record PatientSearchRequestDto(string? SearchTerm, DateTime? DateOfBirth);
