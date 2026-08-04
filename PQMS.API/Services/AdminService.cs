using Microsoft.EntityFrameworkCore;
using PQMS.API.Data;
using PQMS.API.DTOs.Admin;
using PQMS.API.DTOs.Queue;
using PQMS.API.Models;

namespace PQMS.API.Services;

public class AdminService : IAdminService
{
    private readonly PqmsDbContext _context;

    public AdminService(PqmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserListDto>> GetAllUsers()
    {
        return await _context.Users
            .Select(u => new UserListDto(u.Id, u.FullName, u.Email, u.Role, u.IsActive, u.CreatedAt))
            .ToListAsync();
    }

    public async Task UpdateUserRole(int id, UpdateUserRoleDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new Exception("User not found.");

        user.Role = dto.Role;
        user.IsActive = dto.IsActive;
        await _context.SaveChangesAsync();
    }

    public async Task ToggleUserStatus(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new Exception("User not found.");

        user.IsActive = !user.IsActive;
        await _context.SaveChangesAsync();
    }

    public async Task<List<VisitReason>> GetVisitReasons()
    {
        return await _context.VisitReasons.ToListAsync();
    }

    public async Task<VisitReason> AddVisitReason(VisitReason reason)
    {
        _context.VisitReasons.Add(reason);
        await _context.SaveChangesAsync();
        return reason;
    }

    public async Task<VisitReason> UpdateVisitReason(int id, VisitReason reason)
    {
        var existing = await _context.VisitReasons.FindAsync(id);
        if (existing == null)
            throw new Exception("Visit reason not found.");

        existing.Name = reason.Name;
        existing.IsActive = reason.IsActive;
        
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteVisitReason(int id)
    {
        var existing = await _context.VisitReasons.FindAsync(id);
        if (existing == null)
            throw new Exception("Visit reason not found.");

        existing.IsActive = false; // Soft delete
        await _context.SaveChangesAsync();
    }

    public async Task<List<QueueEntry>> GetQueueHistory(QueueHistoryFilterDto filter)
    {
        var query = _context.QueueEntries.Include(q => q.Patient).AsQueryable();

        if (filter.StartDate.HasValue)
            query = query.Where(q => q.QueueDate >= filter.StartDate.Value.Date);
            
        if (filter.EndDate.HasValue)
            query = query.Where(q => q.QueueDate <= filter.EndDate.Value.Date);

        if (!string.IsNullOrEmpty(filter.Status))
            query = query.Where(q => q.Status == filter.Status);

        return await query.OrderByDescending(q => q.CheckInTime).ToListAsync();
    }
}
