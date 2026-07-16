using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Auth;
using Backend.Services;

namespace Backend.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet("stats")]
    [HasPermission("Dashboard.Access")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await dashboardService.GetDashboardStatsAsync();
        return Ok(stats);
    }
}
