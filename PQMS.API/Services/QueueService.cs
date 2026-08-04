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
        var entries = await _context.QueueEntries
            .Include(q => q.Patient)
            .Where(q => q.QueueDate == today)
            .ToListAsync();

        return entries
            .OrderBy(q => new DateTime(q.CheckInTime.Year, q.CheckInTime.Month, q.CheckInTime.Day, q.CheckInTime.Hour, q.CheckInTime.Minute, 0))
            .ThenBy(q => q.CheckInType == "Appointment" ? 0 : 1);
    }

    public async Task<QueueEntry> CallNextPatient()
    {
        var today = DateTime.UtcNow.Date;

        var waitingList = await _context.QueueEntries
            .Where(q => q.QueueDate == today && q.Status == "Waiting")
            .ToListAsync();

        var nextPatient = waitingList
            .OrderBy(q => new DateTime(q.CheckInTime.Year, q.CheckInTime.Month, q.CheckInTime.Day, q.CheckInTime.Hour, q.CheckInTime.Minute, 0))
            .ThenBy(q => q.CheckInType == "Appointment" ? 0 : 1)
            .FirstOrDefault();

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

    public async Task<PQMS.API.DTOs.Queue.CheckInResponseDto> CheckIn(PQMS.API.DTOs.Queue.CheckInRequestDto request)
    {
        var patient = await _context.Patients.FindAsync(request.PatientId);
        if (patient == null)
            throw new Exception("Patient not found.");

        bool alreadyCheckedIn = await _context.QueueEntries
            .AnyAsync(q => q.PatientId == request.PatientId
                        && q.QueueDate == DateTime.UtcNow.Date
                        && q.Status != "Completed");

        if (alreadyCheckedIn)
            throw new Exception("Patient is already in the queue today.");

        if (!request.TermsAccepted)
            throw new Exception("You must accept the terms and conditions.");

        string queueNumber = await GenerateQueueNumber(request.CheckInType, DateTime.UtcNow);

        var entry = new QueueEntry
        {
            PatientId = request.PatientId,
            QueueNumber = queueNumber,
            CheckInType = request.CheckInType,
            VisitReason = request.VisitReason,
            AdditionalInfo = request.AdditionalInfo,
            Status = "Waiting",
            CheckInTime = DateTime.UtcNow,
            QueueDate = DateTime.UtcNow.Date
        };

        _context.QueueEntries.Add(entry);
        await _context.SaveChangesAsync();

        return new PQMS.API.DTOs.Queue.CheckInResponseDto(entry.Id, queueNumber, patient.FullName, "Waiting", entry.CheckInTime);
    }

    public async Task<PQMS.API.DTOs.Queue.QueueDisplayDto> GetQueueDisplay()
    {
        var today = DateTime.UtcNow.Date;

        var currentEntry = await _context.QueueEntries
            .Include(q => q.Patient)
            .Where(q => q.QueueDate == today && q.Status == "InProgress")
            .OrderByDescending(q => q.CalledAt) // en son çağrılan
            .FirstOrDefaultAsync();

        PQMS.API.DTOs.Queue.QueueDisplayItemDto currentPatient = null;
        if (currentEntry != null)
        {
            currentPatient = new PQMS.API.DTOs.Queue.QueueDisplayItemDto(
                currentEntry.QueueNumber,
                currentEntry.Patient?.FullName,
                currentEntry.CheckInTime
            );
        }

        var rawWaitingEntries = await _context.QueueEntries
            .Include(q => q.Patient)
            .Where(q => q.QueueDate == today && q.Status == "Waiting")
            .ToListAsync();

        var waitingEntries = rawWaitingEntries
            .OrderBy(q => new DateTime(q.CheckInTime.Year, q.CheckInTime.Month, q.CheckInTime.Day, q.CheckInTime.Hour, q.CheckInTime.Minute, 0))
            .ThenBy(q => q.CheckInType == "Appointment" ? 0 : 1)
            .Select(q => new PQMS.API.DTOs.Queue.QueueDisplayItemDto(
                q.QueueNumber,
                q.Patient.FullName,
                q.CheckInTime
            ))
            .ToList();

        return new PQMS.API.DTOs.Queue.QueueDisplayDto(currentPatient, waitingEntries);
    }
}
