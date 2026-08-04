using Microsoft.EntityFrameworkCore;
using PQMS.API.Data;
using PQMS.API.DTOs.Appointment;
using PQMS.API.Models;

namespace PQMS.API.Services;

public class AppointmentService : IAppointmentService
{
    private readonly PqmsDbContext _context;

    public AppointmentService(PqmsDbContext context)
    {
        _context = context;
    }

    public async Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto request)
    {
        var patient = await _context.Patients.FindAsync(request.PatientId);
        if (patient == null)
            throw new Exception("Patient not found.");

        var appointment = new Appointment
        {
            PatientId = request.PatientId,
            AppointmentDate = request.AppointmentDate,
            Reason = request.Reason,
            Notes = request.Notes,
            Status = "Scheduled",
            CreatedAt = DateTime.UtcNow
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return new AppointmentDto(
            appointment.Id,
            appointment.PatientId,
            patient.FullName,
            appointment.AppointmentDate,
            appointment.Reason,
            appointment.Status,
            appointment.Notes,
            appointment.CreatedAt
        );
    }

    public async Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(int patientId)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .Select(a => new AppointmentDto(
                a.Id,
                a.PatientId,
                a.Patient.FullName,
                a.AppointmentDate,
                a.Reason,
                a.Status,
                a.Notes,
                a.CreatedAt
            ))
            .ToListAsync();
    }

    public async Task<IEnumerable<AppointmentDto>> GetTodayAppointmentsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        return await _context.Appointments
            .Include(a => a.Patient)
            .Where(a => a.AppointmentDate >= today && a.AppointmentDate < tomorrow)
            .OrderBy(a => a.AppointmentDate)
            .Select(a => new AppointmentDto(
                a.Id,
                a.PatientId,
                a.Patient.FullName,
                a.AppointmentDate,
                a.Reason,
                a.Status,
                a.Notes,
                a.CreatedAt
            ))
            .ToListAsync();
    }
}
