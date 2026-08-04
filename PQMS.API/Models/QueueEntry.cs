using System.ComponentModel.DataAnnotations;

namespace PQMS.API.Models;

public class QueueEntry
{
    public int Id { get; set; }

    [Required]
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    [Required, MaxLength(15)]
    public string QueueNumber { get; set; } = string.Empty; // "WALK-6", "APPT-7"

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Waiting"; // Waiting, InProgress, Completed

    [Required, MaxLength(20)]
    public string CheckInType { get; set; } = "WalkIn"; // WalkIn, Appointment

    [MaxLength(100)]
    public string? VisitReason { get; set; }

    [MaxLength(500)]
    public string? AdditionalInfo { get; set; }

    public DateTime CheckInTime { get; set; } = DateTime.UtcNow;
    public DateTime? CalledAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public DateTime QueueDate { get; set; } = DateTime.UtcNow.Date;
}
