using PQMS.API.DTOs.Appointment;

namespace PQMS.API.Services;

public interface IAppointmentService
{
    Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto request);
    Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(int patientId);
    Task<IEnumerable<AppointmentDto>> GetTodayAppointmentsAsync();
}
