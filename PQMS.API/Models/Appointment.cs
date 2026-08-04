using System.ComponentModel.DataAnnotations;

namespace PQMS.API.Models;

public class Appointment
{
    public int Id { get; set; }

    [Required]
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    [Required]
    public DateTime AppointmentDate { get; set; }

    [MaxLength(100)]
    public string? Reason { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
