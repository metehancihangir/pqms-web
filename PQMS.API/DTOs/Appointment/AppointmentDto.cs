namespace PQMS.API.DTOs.Appointment;

public record AppointmentDto(
    int Id,
    int PatientId,
    string PatientName,
    DateTime AppointmentDate,
    string? Reason,
    string Status,
    string? Notes,
    DateTime CreatedAt
);
