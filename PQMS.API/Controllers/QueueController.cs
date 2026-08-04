using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PQMS.API.Services;

namespace PQMS.API.Controllers;

[ApiController]
[Route("api/queue")]
[Authorize(Roles = "Doctor,Admin")]
public class QueueController : ControllerBase
{
    private readonly IQueueService _queueService;

    public QueueController(IQueueService queueService)
    {
        _queueService = queueService;
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetTodayQueue()
    {
        var queue = await _queueService.GetTodayQueue();
        
        var result = queue.Select(q => new
        {
            id = q.Id,
            patientName = q.Patient?.FullName,
            queueNumber = q.QueueNumber,
            status = q.Status,
            checkInType = q.CheckInType,
            visitReason = q.VisitReason,
            checkInTime = q.CheckInTime
        });

        return Ok(result);
    }

    [HttpPost("checkin")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckIn([FromBody] PQMS.API.DTOs.Queue.CheckInRequestDto request)
    {
        try
        {
            var result = await _queueService.CheckIn(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [HttpPost("call-next")]
    public async Task<IActionResult> CallNextPatient()
    {
        try
        {
            var nextPatient = await _queueService.CallNextPatient();
            return Ok(new { message = "Patient called successfully.", patientId = nextPatient.PatientId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/complete")]
    public async Task<IActionResult> CompletePatient(int id)
    {
        try
        {
            var completedPatient = await _queueService.CompletePatient(id);
            return Ok(new { message = "Patient visit completed.", patientId = completedPatient.PatientId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("display")]
    [AllowAnonymous]
    public async Task<IActionResult> GetQueueDisplay()
    {
        var displayData = await _queueService.GetQueueDisplay();
        return Ok(displayData);
    }
}
