using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PQMS.API.DTOs.Patient;
using PQMS.API.Services;

namespace PQMS.API.Controllers;

[ApiController]
[Route("api/patients")]
// [Authorize] -> Kiosk ve resepsiyon için token bazlı authorization stratejisi projenin yapısına göre belirlenebilir. Şimdilik public bırakalım veya daha sonra Authorize eklenebilir.
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? searchTerm, [FromQuery] DateTime? dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) && !dateOfBirth.HasValue)
        {
            return BadRequest(new { message = "Lütfen arama terimi veya doğum tarihi giriniz." });
        }

        var results = await _patientService.SearchPatients(searchTerm, dateOfBirth);
        return Ok(results);
    }
}
