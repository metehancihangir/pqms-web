namespace PQMS.API.DTOs.Queue;

public record CheckInResponseDto(
    int QueueEntryId,
    string QueueNumber,
    string PatientName,
    string Status,
    DateTime CheckInTime
);
