using System.ComponentModel.DataAnnotations;

namespace PQMS.API.DTOs.Appointment;

public record CreateAppointmentDto(
    [Required] int PatientId,
    [Required] DateTime AppointmentDate,
    [MaxLength(100)] string? Reason,
    [MaxLength(500)] string? Notes
);
