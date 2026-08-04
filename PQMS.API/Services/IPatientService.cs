using PQMS.API.DTOs.Patient;

namespace PQMS.API.Services;

public interface IPatientService
{
    Task<List<PatientSearchResultDto>> SearchPatients(string? searchTerm, DateTime? dateOfBirth);
}
