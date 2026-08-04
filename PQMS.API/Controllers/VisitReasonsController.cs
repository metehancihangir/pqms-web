using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PQMS.API.Data;
using PQMS.API.Models;

namespace PQMS.API.Controllers;

[ApiController]
[Route("api/visit-reasons")]
public class VisitReasonsController : ControllerBase
{
    private readonly PqmsDbContext _context;

    public VisitReasonsController(PqmsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetVisitReasons()
    {
        var reasons = await _context.VisitReasons
            .Where(v => v.IsActive)
            .OrderBy(v => v.Id)
            .ToListAsync();
        return Ok(reasons);
    }
}
