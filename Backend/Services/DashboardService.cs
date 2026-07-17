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

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(string filter = "monthly")
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
        var now = DateTime.UtcNow;

        if (filter == "daily")
        {
            for (int i = 0; i < 7; i++)
            {
                var day = now.AddDays(-i);
                chartLabels.Add(day.ToString("dd MMM"));
                var count = await _context.Users.CountAsync(u => u.CreatedAt.Date == day.Date);
                chartValues.Add(count);
            }
        }
        else if (filter == "weekly")
        {
            for (int i = 0; i < 4; i++)
            {
                var startOfWeek = now.AddDays(-((int)now.DayOfWeek) - (i * 7)).Date;
                var endOfWeek = startOfWeek.AddDays(7).AddTicks(-1);
                chartLabels.Add($"{startOfWeek:dd MMM} - {endOfWeek:dd MMM}");
                var count = await _context.Users.CountAsync(u => u.CreatedAt >= startOfWeek && u.CreatedAt <= endOfWeek);
                chartValues.Add(count);
            }
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                var month = now.AddMonths(-i);
                chartLabels.Add(month.ToString("MMM yyyy"));
                var count = await _context.Users.CountAsync(u => u.CreatedAt.Month == month.Month && u.CreatedAt.Year == month.Year);
                chartValues.Add(count);
            }
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