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
        var result = await shortLinkService.ResolveAndTrackClickAsync(shortCode);
        return result.Status switch
        {
            ResultStatus.Success => Ok(new { originalUrl = result.Data }),
            ResultStatus.Expired => NotFound(new { message = "Bu linkin süresi dolmuş." }),
            _ => NotFound(new { message = "Link bulunamadı." })
        };
    }
}