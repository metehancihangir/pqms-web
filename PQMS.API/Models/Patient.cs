using System.ComponentModel.DataAnnotations;

namespace PQMS.API.Models;

public class Patient
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<QueueEntry> QueueEntries { get; set; } = new List<QueueEntry>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
