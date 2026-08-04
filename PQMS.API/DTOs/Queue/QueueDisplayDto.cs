namespace PQMS.API.DTOs.Queue;

public record QueueDisplayDto(
    QueueDisplayItemDto? CurrentPatient,
    List<QueueDisplayItemDto> WaitingList
);
