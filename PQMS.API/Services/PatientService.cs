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

    public async Task<PatientSearchResultDto> CreatePatient(PatientCreateDto dto)
    {
        var existing = await _context.Patients
            .AnyAsync(p => p.FullName == dto.FullName && p.DateOfBirth == dto.DateOfBirth);

        if (existing)
            throw new Exception("A patient with this name and date of birth already exists.");

        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            var existingPhone = await _context.Patients.AnyAsync(p => p.PhoneNumber == dto.PhoneNumber);
            if (existingPhone)
                throw new Exception("This phone number is already registered.");
        }

        var patient = new PQMS.API.Models.Patient
        {
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            DateOfBirth = dto.DateOfBirth,
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return new PatientSearchResultDto(patient.Id, patient.FullName, patient.PhoneNumber, patient.DateOfBirth);
    }
}
