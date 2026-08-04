namespace PQMS.API.DTOs.Queue;

public record CheckInRequestDto(
    int PatientId,
    string CheckInType,
    string VisitReason,
    string? AdditionalInfo,
    bool TermsAccepted
);
