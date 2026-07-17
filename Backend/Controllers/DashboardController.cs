using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Auth;
using Backend.Services;

namespace Backend.Controllers;

[ApiController]
[Authorize]
[Route("api/Dashboard")]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet("stats")]
    [HasPermission("Dashboard.Access")]
    public async Task<IActionResult> GetStats([FromQuery] string filter = "monthly")
    {
        var stats = await dashboardService.GetDashboardStatsAsync(filter);
        return Ok(stats);
    }
}
