using PQMS.API.DTOs.Admin;
using PQMS.API.DTOs.Queue;
using PQMS.API.Models;

namespace PQMS.API.Services;

public interface IAdminService
{
    // User Management
    Task<List<UserListDto>> GetAllUsers();
    Task UpdateUserRole(int id, UpdateUserRoleDto dto);
    Task ToggleUserStatus(int id);

    // Visit Reasons Management
    Task<List<VisitReason>> GetVisitReasons();
    Task<VisitReason> AddVisitReason(VisitReason reason);
    Task<VisitReason> UpdateVisitReason(int id, VisitReason reason);
    Task DeleteVisitReason(int id);

    // Queue History
    Task<List<QueueEntry>> GetQueueHistory(QueueHistoryFilterDto filter);
}
