using Backend.Auth;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Backend.Controllers;


[ApiController]
[AllowAnonymous]
[Route("api/links")]
public class RedirectController(IShortLinkService shortLinkService) : ControllerBase
{
    [HttpGet("resolve/{shortCode}")]
    [EnableRateLimiting(RateLimitPolicies.LinkRedirect)]
    public async Task<IActionResult> ResolveShortCode(string shortCode)
    {
        var targetUrl = await shortLinkService.ResolveAndTrackClickAsync(shortCode);
        if (targetUrl is null) return NotFound(new { message = "Link bulunamadı." });
        return Ok(new { targetUrl });
    }
}