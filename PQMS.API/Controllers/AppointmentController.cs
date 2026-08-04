using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PQMS.API.DTOs.Appointment;
using PQMS.API.Services;

namespace PQMS.API.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto request)
    {
        try
        {
            var result = await _appointmentService.CreateAppointmentAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("patient/{patientId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPatientAppointments(int patientId)
    {
        var appointments = await _appointmentService.GetPatientAppointmentsAsync(patientId);
        return Ok(appointments);
    }
}
