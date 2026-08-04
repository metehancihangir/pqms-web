using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PQMS.API.DTOs.Admin;
using PQMS.API.DTOs.Queue;
using PQMS.API.Models;
using PQMS.API.Services;

namespace PQMS.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    // ===== User Management =====
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await _adminService.GetAllUsers());
    }

    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDto dto)
    {
        try
        {
            await _adminService.UpdateUserRole(id, dto);
            return Ok(new { message = "User role updated successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("users/{id}/status")]
    public async Task<IActionResult> ToggleUserStatus(int id)
    {
        try
        {
            await _adminService.ToggleUserStatus(id);
            return Ok(new { message = "User status toggled successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ===== Visit Reasons Management =====
    [HttpGet("visit-reasons")]
    public async Task<IActionResult> GetVisitReasons()
    {
        return Ok(await _adminService.GetVisitReasons());
    }

    [HttpPost("visit-reasons")]
    public async Task<IActionResult> AddVisitReason([FromBody] VisitReason reason)
    {
        var result = await _adminService.AddVisitReason(reason);
        return StatusCode(201, result);
    }

    [HttpPut("visit-reasons/{id}")]
    public async Task<IActionResult> UpdateVisitReason(int id, [FromBody] VisitReason reason)
    {
        try
        {
            var result = await _adminService.UpdateVisitReason(id, reason);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("visit-reasons/{id}")]
    public async Task<IActionResult> DeleteVisitReason(int id)
    {
        try
        {
            await _adminService.DeleteVisitReason(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ===== Queue History =====
    [HttpGet("queue-history")]
    public async Task<IActionResult> GetQueueHistory([FromQuery] QueueHistoryFilterDto filter)
    {
        var history = await _adminService.GetQueueHistory(filter);
        
        // Include PatientName since UI expects it
        var result = history.Select(q => new
        {
            id = q.Id,
            queueNumber = q.QueueNumber,
            patientName = q.Patient?.FullName,
            checkInType = q.CheckInType,
            visitReason = q.VisitReason,
            status = q.Status,
            queueDate = q.QueueDate,
            checkInTime = q.CheckInTime,
            calledAt = q.CalledAt,
            completedAt = q.CompletedAt
        });

        return Ok(result);
    }
}
