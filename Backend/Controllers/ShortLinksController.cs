using System.Security.Claims;
using Backend.Auth;
using Backend.DTOs.Links;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Backend.Controllers;

[ApiController]
[Authorize]
[Route("api/links")]
public class ShortLinksController(IShortLinkService shortLinkService) : ControllerBase
{
    private int GetUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpPost("my")]
    [HasPermission("Links.Create")]
    [EnableRateLimiting(RateLimitPolicies.LinkCreate)]
    public async Task<IActionResult> CreateShortLink([FromBody] ShortLinkCreateRequest request)
    {
        var result = await shortLinkService.CreateShortLinkAsync(request, GetUserId());
        return result.Status switch
        {
            ResultStatus.Success => Ok(result.Data),
            ResultStatus.Invalid => BadRequest(new { message = result.Message }),
            ResultStatus.Conflict => Conflict(new { message = result.Message }),
            _ => StatusCode(500)
        };
    }
    [HttpGet("my")]
    [HasPermission("Links.View")]
    public async Task<IActionResult> GetMyLinksSummary()
    {
        var result = await shortLinkService.GetMyLinksSummaryAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("my/chart")]
    [HasPermission("Links.View")]
    public async Task<IActionResult> GetMyLinksChartData()
    {
        var result = await shortLinkService.GetClickChartAsync(GetUserId());
        return Ok(result);
    }




    [HttpPut("my/{id:long}")]
    [HasPermission("Links.Edit")]
    public async Task<IActionResult> UpdateMyLink(long id, ShortLinkUpdateRequest request)
    {
        var result = await shortLinkService.UpdateShortLinkAsync(id, request, GetUserId(), canManageAll: false);
        return result.Status switch
        {
            ResultStatus.Success => Ok(result.Data),
            ResultStatus.NotFound => NotFound(),
            ResultStatus.Forbidden => Forbid(),
            ResultStatus.Invalid => BadRequest(new { message = result.Message }),
            _ => StatusCode(500)
        };
    }

    [HttpDelete("my/{id:long}")]
    [HasPermission("Links.Delete")]
    public async Task<IActionResult> DeleteShortLink([FromRoute] long id)
    {
        var result = await shortLinkService.DeleteShortLinkAsync(id, GetUserId(), false);
        return result.Status switch
        {
            ResultStatus.Success => NoContent(),
            ResultStatus.NotFound => NotFound(),
            ResultStatus.Forbidden => Forbid(),
            _ => StatusCode(500)
        };
    }
    [HttpGet("admin")]
    [HasPermission("Links.ManageAll")]
    public async Task<IActionResult> GetAdminLinks([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await shortLinkService.GetAdminLinksAsync(search, page, pageSize);
        return Ok(result);
    }
    [HttpDelete("admin/{id:long}")]
    [HasPermission("Links.ManageAll")]
    public async Task<IActionResult> DeleteAdminLink([FromRoute] long id)
    {
        var result = await shortLinkService.DeleteShortLinkAsync(id, GetUserId(), true);
        return result.Status switch
        {
            ResultStatus.Success => NoContent(),
            ResultStatus.NotFound => NotFound(),
            ResultStatus.Forbidden => Forbid(),
            _ => StatusCode(500)
        };
    }
}