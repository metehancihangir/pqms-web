using PQMS.API.Models;

namespace PQMS.API.Services;

public interface IQueueService
{
    Task<string> GenerateQueueNumber(string checkInType, DateTime date);
    Task<IEnumerable<QueueEntry>> GetTodayQueue();
    Task<QueueEntry> CallNextPatient();
    Task<QueueEntry> CompletePatient(int queueId);
}
