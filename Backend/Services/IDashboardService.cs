using Backend.DTOs;

namespace Backend.Services;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync(string filter = "monthly", DateTime? startDate = null, DateTime? endDate = null);
}