using PQMS.API.Models;

namespace PQMS.API.Services;

public interface IQueueService
{
    Task<string> GenerateQueueNumber(string checkInType, DateTime date);
    Task<IEnumerable<QueueEntry>> GetTodayQueue();
    Task<QueueEntry> CallNextPatient();
    Task<PQMS.API.DTOs.Queue.CheckInResponseDto> CheckIn(PQMS.API.DTOs.Queue.CheckInRequestDto request);
    Task<QueueEntry> CompletePatient(int queueId);
    Task<PQMS.API.DTOs.Queue.QueueDisplayDto> GetQueueDisplay();
}
