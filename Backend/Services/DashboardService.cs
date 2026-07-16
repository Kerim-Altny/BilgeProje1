using System.Globalization;
using Backend.Data;
using Backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class DashboardService : IDashboardService
{
    private const int MonthlyGrowthWindow = 6;
    private const int RecentUsersCount = 5;

    private static readonly CultureInfo TurkishCulture = new("tr-TR");

    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsResponse> GetDashboardStatsAsync()
    {
        var now = DateTime.UtcNow;
        var currentMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var windowStart = currentMonthStart.AddMonths(-(MonthlyGrowthWindow - 1));

        var totalUsers = await _context.Users.CountAsync();
        var totalRoles = await _context.Roles.CountAsync();

        var newUsersThisMonth = await _context.Users
            .CountAsync(u => u.CreatedAt >= currentMonthStart);

        var creationDatesInWindow = await _context.Users
            .Where(u => u.CreatedAt >= windowStart)
            .Select(u => u.CreatedAt)
            .ToListAsync();

        var monthlyUserGrowth = new List<MonthlyGrowthItem>();
        for (var offset = MonthlyGrowthWindow - 1; offset >= 0; offset--)
        {
            var monthStart = currentMonthStart.AddMonths(-offset);
            var userCount = creationDatesInWindow
                .Count(date => date.Year == monthStart.Year && date.Month == monthStart.Month);

            monthlyUserGrowth.Add(new MonthlyGrowthItem
            {
                Month = monthStart.ToString("MMM yyyy", TurkishCulture),
                UserCount = userCount
            });
        }

        var recentUsers = await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .Take(RecentUsersCount)
            .Select(u => new RecentUserItem
            {
                Username = u.Username,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return new DashboardStatsResponse
        {
            TotalUsers = totalUsers,
            TotalRoles = totalRoles,
            NewUsersThisMonth = newUsersThisMonth,
            MonthlyUserGrowth = monthlyUserGrowth,
            RecentUsers = recentUsers
        };
    }

}
