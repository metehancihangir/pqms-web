using Microsoft.EntityFrameworkCore;
using PQMS.API.Data;
using PQMS.API.Models;

namespace PQMS.API.Services;

public class QueueService : IQueueService
{
    private readonly PqmsDbContext _context;

    public QueueService(PqmsDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateQueueNumber(string checkInType, DateTime date)
    {
        string prefix = checkInType == "Appointment" ? "APPT" : "WALK";

        int todayCount = await _context.QueueEntries
            .Where(q => q.QueueDate == date.Date && q.CheckInType == checkInType)
            .CountAsync();

        return $"{prefix}-{todayCount + 1}";
    }

    public async Task<IEnumerable<QueueEntry>> GetTodayQueue()
    {
        var today = DateTime.UtcNow.Date;
        return await _context.QueueEntries
            .Include(q => q.Patient)
            .Where(q => q.QueueDate == today)
            .OrderBy(q => q.CheckInTime)
            .ToListAsync();
    }

    public async Task<QueueEntry> CallNextPatient()
    {
        var today = DateTime.UtcNow.Date;

        var nextPatient = await _context.QueueEntries
            .Where(q => q.QueueDate == today && q.Status == "Waiting")
            .OrderBy(q => q.CheckInTime)
            .FirstOrDefaultAsync();

        if (nextPatient == null)
            throw new Exception("No waiting patients found.");

        // Complete the currently in-progress patient if any
        var currentInProgress = await _context.QueueEntries
            .Where(q => q.QueueDate == today && q.Status == "InProgress")
            .FirstOrDefaultAsync();

        if (currentInProgress != null)
        {
            currentInProgress.Status = "Completed";
            currentInProgress.CompletedAt = DateTime.UtcNow;
        }

        nextPatient.Status = "InProgress";
        nextPatient.CalledAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return nextPatient;
    }

    public async Task<QueueEntry> CompletePatient(int queueId)
    {
        var queueEntry = await _context.QueueEntries.FindAsync(queueId);
        
        if (queueEntry == null)
            throw new Exception("Queue entry not found.");

        if (queueEntry.Status != "InProgress")
            throw new Exception("Patient is not currently in progress.");

        queueEntry.Status = "Completed";
        queueEntry.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return queueEntry;
    }
}
