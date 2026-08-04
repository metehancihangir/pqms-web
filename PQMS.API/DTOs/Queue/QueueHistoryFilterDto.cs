namespace PQMS.API.DTOs.Queue;

public record QueueHistoryFilterDto(
    DateTime? StartDate, 
    DateTime? EndDate, 
    string? Status
);
