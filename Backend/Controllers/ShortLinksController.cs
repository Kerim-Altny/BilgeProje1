

using System.Security.Claims;
using Backend.Auth;
using Backend.DTOs.Links;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Authorize]
[Route("api/urls")]
public class ShortLinksController(IShortLinkService shortLinkService) : ControllerBase
{
    [HttpPost("shortlinks")]
    [HasPermission("ShortLinks.Create")]
    public async Task<IActionResult> CreateShortLink(ShortLinkCreateRequest createRequest)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await shortLinkService.CreateShortLinkAsync(createRequest, userId);
        return result.Status switch
        {
            ResultStatus.Success => CreatedAtAction(nameof(GetShortLinkById), new { id = result.Data!.Id }, result.Data),
            ResultStatus.Invalid => BadRequest(new { message = result.Message }),
            ResultStatus.Conflict => Conflict(new { message = result.Message }),
            _ => StatusCode(500)
        };
    }
}