using Backend.Auth;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Authorize]
[Route("api/links")]
public class LinksController(AppDbContext context) : ControllerBase
{
    // ─── ADMIN ──────────────────────────────────────────────────────────────

    // GET /api/links/admin — Tüm kullanıcıların linklerini istatistiklerle döner
    [HttpGet("admin")]
    [HasPermission("Links.View")]
    public async Task<IActionResult> GetAllLinks(
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = context.ShortLinks
            .Include(l => l.CreatedByUser)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(l =>
                l.TargetUrl.Contains(search) ||
                (l.CreatedByUser != null && l.CreatedByUser.Username.Contains(search)));
        }

        var totalLinks = await query.CountAsync();
        var totalClicks = await query.SumAsync(l => l.ClickCount);
        var totalUsers = await query.Select(l => l.CreatedByUserId).Distinct().CountAsync();

        var rawLinks = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new
            {
                l.Id,
                l.TargetUrl,
                l.ClickCount,
                l.CreatedAt,
                l.ExpirationDate,
                Username = l.CreatedByUser != null ? l.CreatedByUser.Username : "Bilinmiyor",
                l.CreatedByUserId
            })
            .ToListAsync();

        var links = rawLinks.Select(l => new AdminLinkResponse
        {
            Id = l.Id,
            TargetUrl = l.TargetUrl,
            ShortCode = EncodeBase62(l.Id),
            ClickCount = l.ClickCount,
            CreatedAt = l.CreatedAt,
            ExpirationDate = l.ExpirationDate,
            Username = l.Username,
            UserId = l.CreatedByUserId
        }).ToList();

        return Ok(new AdminLinksStatsResponse
        {
            TotalLinks = totalLinks,
            TotalClicks = totalClicks,
            TotalUsers = totalUsers,
            Links = links
        });
    }

    // DELETE /api/links/admin/{id} — Admin bir linki silebilir
    [HttpDelete("admin/{id:long}")]
    [HasPermission("Links.Delete")]
    public async Task<IActionResult> AdminDeleteLink(long id)
    {
        var link = await context.ShortLinks.FindAsync(id);
        if (link is null) return NotFound();

        context.ShortLinks.Remove(link);
        await context.SaveChangesAsync();
        return NoContent();
    }

    // ─── USER (kendi linkleri) ───────────────────────────────────────────────

    // POST /api/links/my — Yeni kısa link oluştur
    [HttpPost("my")]
    public async Task<IActionResult> CreateLink([FromBody] CreateLinkRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized();

        if (string.IsNullOrWhiteSpace(request.TargetUrl))
            return BadRequest(new { message = "URL boş olamaz." });

        // Benzersiz ID: ticks bazlı (Snowflake yerine geçici)
        var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000 + new Random().Next(0, 999);
        var shortCode = EncodeBase62(id);

        var link = new ShortLink
        {
            Id = id,
            TargetUrl = request.TargetUrl,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            ExpirationDate = request.ExpirationDate,
            ClickCount = 0
        };

        context.ShortLinks.Add(link);
        await context.SaveChangesAsync();

        return Ok(new
        {
            id = link.Id,
            shortCode,
            originalUrl = link.TargetUrl,
            createdAt = link.CreatedAt
        });
    }

    // GET /api/links/my — Giriş yapmış kullanıcının kendi linklerini döner
    [HttpGet("my")]
    public async Task<IActionResult> GetMyLinks([FromQuery] string? search = null)
    {
        var userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized();

        var query = context.ShortLinks
            .Where(l => l.CreatedByUserId == userId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(l => l.TargetUrl.Contains(search));

        var totalLinks = await query.CountAsync();
        var totalClicks = await query.SumAsync(l => l.ClickCount);
        var topLink = await query
            .OrderByDescending(l => l.ClickCount)
            .Select(l => l.TargetUrl)
            .FirstOrDefaultAsync();

        var rawLinks2 = await query
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new
            {
                l.Id,
                l.TargetUrl,
                l.ClickCount,
                l.CreatedAt,
                l.ExpirationDate
            })
            .ToListAsync();

        var links = rawLinks2.Select(l => new
        {
            id = l.Id,
            originalUrl = l.TargetUrl,
            shortCode = EncodeBase62(l.Id),
            clickCount = l.ClickCount,
            createdAt = l.CreatedAt,
            expirationDate = l.ExpirationDate
        }).ToList();

        return Ok(new
        {
            totalLinks,
            totalClicks,
            topLink,
            links
        });
    }

    // GET /api/links/my/chart — Son 7 günlük tıklama verisi (grafik için)
    [HttpGet("my/chart")]
    public async Task<IActionResult> GetMyClickChart()
    {
        var userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized();

        var today = DateTime.UtcNow.Date;
        var sevenDaysAgo = today.AddDays(-6);

        // Kullanıcının tüm linkleri (tıklama sayısı ile birlikte)
        var links = await context.ShortLinks
            .Where(l => l.CreatedByUserId == userId)
            .Select(l => new { l.CreatedAt, l.ClickCount })
            .ToListAsync();

        // Son 7 gün için: linkin oluşturulduğu güne göre grupla
        var grouped = links
            .Where(l => l.CreatedAt.Date >= sevenDaysAgo)
            .GroupBy(l => l.CreatedAt.Date)
            .ToDictionary(g => g.Key, g => g.Sum(l => l.ClickCount));

        // 7 günün tümünü sırayla doldur (veri yoksa 0)
        var result = Enumerable.Range(0, 7).Select(i =>
        {
            var date = sevenDaysAgo.AddDays(i);
            return new
            {
                date = date.ToString("dd.MM"),
                dayName = date.DayOfWeek switch
                {
                    DayOfWeek.Monday => "Pzt",
                    DayOfWeek.Tuesday => "Sal",
                    DayOfWeek.Wednesday => "Çar",
                    DayOfWeek.Thursday => "Per",
                    DayOfWeek.Friday => "Cum",
                    DayOfWeek.Saturday => "Cmt",
                    DayOfWeek.Sunday => "Paz",
                    _ => ""
                },
                clicks = grouped.TryGetValue(date, out var c) ? c : 0
            };
        }).ToList();

        return Ok(result);
    }

    // DELETE /api/links/my/{id} — Kullanıcı kendi linkini silebilir
    [HttpDelete("my/{id:long}")]
    public async Task<IActionResult> DeleteMyLink(long id)
    {
        var userId = GetCurrentUserId();
        var link = await context.ShortLinks.FindAsync(id);

        if (link is null) return NotFound();
        if (link.CreatedByUserId != userId) return Forbid();

        context.ShortLinks.Remove(link);
        await context.SaveChangesAsync();
        return NoContent();
    }

    // ─── REDIRECT ────────────────────────────────────────────────────────────

    // GET /api/links/resolve/{code} — Kodu çöz, tıklamayı say, hedef URL'i döndür
    [HttpGet("resolve/{code}")]
    [AllowAnonymous]
    public async Task<IActionResult> Resolve(string code)
    {
        // Önce Base62 dene, başarısız olursa hex dene (eski linklere uyumluluk)
        long id = DecodeBase62(code);

        // Base62 bulamadıysa hex dene
        if (id <= 0 && long.TryParse(code, System.Globalization.NumberStyles.HexNumber, null, out var hexId))
            id = hexId;

        if (id <= 0) return NotFound(new { message = "Geçersiz kısa link." });

        // Linki bul
        var link = await context.ShortLinks
            .AsNoTracking()
            .Where(l => l.Id == id)
            .Select(l => new { l.TargetUrl, l.ExpirationDate })
            .FirstOrDefaultAsync();

        if (link is null) return NotFound(new { message = "Link bulunamadı." });

        if (link.ExpirationDate.HasValue && link.ExpirationDate.Value < DateTime.UtcNow)
            return BadRequest(new { message = "Bu linkin süresi dolmuş." });

        // Tıklamayı atomik olarak artır (race condition olmaz)
        await context.ShortLinks
            .Where(l => l.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(l => l.ClickCount, l => l.ClickCount + 1));

        return Ok(new { targetUrl = link.TargetUrl });
    }

    // ─── HELPERS ────────────────────────────────────────────────────────────

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                 ?? User.FindFirst("sub")
                 ?? User.FindFirst("nameid");
        return int.TryParse(claim?.Value, out var id) ? id : 0;
    }

    private static string EncodeBase62(long value)
    {
        const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        if (value == 0) return "0";
        var result = new System.Text.StringBuilder();
        while (value > 0)
        {
            result.Insert(0, chars[(int)(value % 62)]);
            value /= 62;
        }
        return result.ToString();
    }

    private static long DecodeBase62(string code)
    {
        const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        long result = 0;
        foreach (var c in code)
        {
            var idx = chars.IndexOf(c);
            if (idx < 0) return -1;
            result = result * 62 + idx;
        }
        return result;
    }
}

public class CreateLinkRequest
{
    public string TargetUrl { get; set; } = string.Empty;
    public DateTime? ExpirationDate { get; set; }
}
