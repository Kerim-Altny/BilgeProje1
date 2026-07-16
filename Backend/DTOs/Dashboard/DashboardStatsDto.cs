namespace Backend.DTOs;

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalRoles { get; set; }
    public int UsersLast30Days { get; set; }
    public List<UserResponse> RecentUsers { get; set; } = new List<UserResponse>();
    public List<string> ChartLabels { get; set; } = new List<string>();
    public List<int> ChartValues { get; set; } = new List<int>();
}