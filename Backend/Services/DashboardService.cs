using AutoMapper;
using Backend.Data;
using Backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public DashboardService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var totalUsers = await _context.Users.CountAsync();
        var totalRoles = await _context.Roles.CountAsync();
        var usersLast30Days = await _context.Users.CountAsync(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-30));
        var recentUsers = await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .Take(5)
            .ToListAsync();

        var chartLabels = new List<string>();
        var chartValues = new List<int>();

        for (int i = 0; i < 6; i++)
        {
            var month = DateTime.UtcNow.AddMonths(-i);
            var monthLabel = month.ToString("MMM yyyy");
            chartLabels.Add(monthLabel);

            var userCount = await _context.Users.CountAsync(u => u.CreatedAt.Month == month.Month && u.CreatedAt.Year == month.Year);
            chartValues.Add(userCount);
        }

        chartLabels.Reverse();
        chartValues.Reverse();

        return new DashboardStatsDto
        {
            TotalUsers = totalUsers,
            TotalRoles = totalRoles,
            UsersLast30Days = usersLast30Days,
            RecentUsers = _mapper.Map<List<UserResponse>>(recentUsers),
            ChartLabels = chartLabels,
            ChartValues = chartValues
        };
    }
}