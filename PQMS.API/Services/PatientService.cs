using Microsoft.EntityFrameworkCore;
using PQMS.API.Data;
using PQMS.API.DTOs.Patient;

namespace PQMS.API.Services;

public class PatientService : IPatientService
{
    private readonly PqmsDbContext _context;

    public PatientService(PqmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<PatientSearchResultDto>> SearchPatients(string? searchTerm, DateTime? dateOfBirth)
    {
        var query = _context.Patients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string term = searchTerm.Trim().ToLower();
            query = query.Where(p =>
                p.FullName.ToLower().Contains(term) ||
                (p.PhoneNumber != null && p.PhoneNumber.Contains(term))
            );
        }

        if (dateOfBirth.HasValue)
        {
            query = query.Where(p => p.DateOfBirth == dateOfBirth.Value.Date);
        }

        return await query
            .Select(p => new PatientSearchResultDto(p.Id, p.FullName, p.PhoneNumber, p.DateOfBirth))
            .Take(20)
            .ToListAsync();
    }
}
