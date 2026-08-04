namespace PQMS.API.DTOs.Queue;

public record QueueDisplayItemDto(
    string QueueNumber,
    string PatientName,
    DateTime CheckInTime
);
