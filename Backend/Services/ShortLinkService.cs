using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Backend.DTOs.Links;
namespace Backend.Services;


public class ShortLinkService : IShortLinkService
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ISnowflakeIdGenerator _idGenerator;
    private readonly IUrlSafetyValidator _urlSafetyValidator;

    public ShortLinkService(AppDbContext dbContext, IMapper mapper, ISnowflakeIdGenerator idGenerator, IUrlSafetyValidator urlSafetyValidator)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _idGenerator = idGenerator;
        _urlSafetyValidator = urlSafetyValidator;
    }

    public async Task<Result<ShortLinkResponse>> CreateShortLinkAsync(ShortLinkCreateRequest shortLinkCreateRequest, int userId)
    {
        var urlvalidation = _urlSafetyValidator.Validate(shortLinkCreateRequest.OriginalUrl);
        if (!urlvalidation.IsValid)
        {
            return Result<ShortLinkResponse>.Invalid(urlvalidation.ErrorMessage);
        }
        if (!string.IsNullOrWhiteSpace(shortLinkCreateRequest.CustomCode))
        {
            if (await _dbContext.ShortLinks.AnyAsync(sl => sl.ShortCode == shortLinkCreateRequest.CustomCode))
            {
                return Result<ShortLinkResponse>.Conflict("Bu özel kod zaten kullanılıyor.");
            }
        }
        var newId = _idGenerator.NextId();
        var newCustomCode = shortLinkCreateRequest.CustomCode ?? Base62Encoder.Encode(newId);

        var newShortLink = new ShortLink
        {
            Id = newId,
            OriginalUrl = urlvalidation.NormalizedUrl!,
            ShortCode = newCustomCode,
            CreatedByUserId = userId,
        };
        _dbContext.ShortLinks.Add(newShortLink);
        await _dbContext.SaveChangesAsync();

        return Result<ShortLinkResponse>.Ok(_mapper.Map<ShortLinkResponse>(newShortLink));
    }
    public async Task<MyLinksSummaryResponse> GetMyLinksSummaryAsync(int userId)
    {
        var query = _dbContext.ShortLinks.Where(sl => sl.CreatedByUserId == userId);

        var totalLinks = await query.CountAsync();
        var totalClicks = await query.SumAsync(sl => sl.ClickCount);
        var topLink = await query
            .OrderByDescending(sl => sl.ClickCount)
            .Select(sl => sl.OriginalUrl)
            .FirstOrDefaultAsync();

        var links = await query.OrderByDescending(sl => sl.CreatedAt).ToListAsync();

        return new MyLinksSummaryResponse
        {
            TotalLinks = totalLinks,
            TotalClicks = totalClicks,
            TopLink = topLink,
            Links = links.Select(_mapper.Map<ShortLinkResponse>).ToList()
        };
    }

    public async Task<IReadOnlyList<ClickChartPointResponse>> GetClickChartAsync(int userId)
    {
        var today = DateTime.UtcNow.Date;
        var startDate = today.AddDays(-6);

        var clicks = await _dbContext.ShortLinkClicks
            .Include(c => c.ShortLink)
            .Where(c => c.ShortLink!.CreatedByUserId == userId && c.ClickedAt.Date >= startDate)
            .Select(c => new { c.ClickedAt })
            .ToListAsync();

        var grouped = clicks
            .GroupBy(c => c.ClickedAt.Date)
            .ToDictionary(g => g.Key, g => (long)g.Count());

        var result = new List<ClickChartPointResponse>();
        for (var i = 0; i < 7; i++)
        {
            var date = startDate.AddDays(i);
            result.Add(new ClickChartPointResponse
            {
                Date = date.ToString("dd.MM"),
                DayName = date.DayOfWeek switch
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
                Clicks = grouped.TryGetValue(date, out var clicksCount) ? clicksCount : 0
            });
        }

        return result;
    }

    public async Task<AdminLinksListResponse> GetAdminLinksAsync(string? search, int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _dbContext.ShortLinks.Include(sl => sl.CreatedByUser).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(sl =>
                sl.OriginalUrl.Contains(search) ||
                (sl.CreatedByUser != null && sl.CreatedByUser.Username.Contains(search)));
        }

        var totalLinks = await query.CountAsync();
        var totalClicks = await query.SumAsync(sl => sl.ClickCount);
        var totalUsers = await query.Select(sl => sl.CreatedByUserId).Distinct().CountAsync();

        var links = await query
            .OrderByDescending(sl => sl.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new AdminLinksListResponse
        {
            TotalLinks = totalLinks,
            TotalClicks = totalClicks,
            TotalUsers = totalUsers,
            Links = links.Select(_mapper.Map<AdminLinkResponse>).ToList()
        };
    }

    public async Task<Result<ShortLinkResponse>> UpdateShortLinkAsync(long shortLinkId, ShortLinkUpdateRequest shortLinkUpdateRequest, int userId, bool canManageAll)
    {
        var shortLink = await _dbContext.ShortLinks.FirstOrDefaultAsync(sl => sl.Id == shortLinkId);
        if (shortLink is null) return Result<ShortLinkResponse>.NotFound();
        if (!canManageAll && shortLink.CreatedByUserId != userId) return Result<ShortLinkResponse>.Forbidden();
        var urlvalidation = _urlSafetyValidator.Validate(shortLinkUpdateRequest.OriginalUrl);
        if (!urlvalidation.IsValid) return Result<ShortLinkResponse>.Invalid(urlvalidation.ErrorMessage);
        shortLink.OriginalUrl = urlvalidation.NormalizedUrl!;
        await _dbContext.SaveChangesAsync();
        return Result<ShortLinkResponse>.Ok(_mapper.Map<ShortLinkResponse>(shortLink));


    }
    public async Task<Result<bool>> DeleteShortLinkAsync(long shortLinkId, int userId, bool canManageAll)
    {
        var shortLink = await _dbContext.ShortLinks.FirstOrDefaultAsync(sl => sl.Id == shortLinkId);
        if (shortLink is null) return Result<bool>.NotFound();
        if (!canManageAll && shortLink.CreatedByUserId != userId) return Result<bool>.Forbidden();
        _dbContext.ShortLinks.Remove(shortLink);
        await _dbContext.SaveChangesAsync();
        return Result<bool>.Ok(true);
    }



    public async Task<Result<string>> ResolveAndTrackClickAsync(string shortLinkCode)
    {
        var shortLink = await _dbContext.ShortLinks.FirstOrDefaultAsync(sl => sl.ShortCode == shortLinkCode);
        if (shortLink is null) return Result<string>.NotFound();

        if (shortLink.ExpirationDate.HasValue && shortLink.ExpirationDate.Value < DateTime.UtcNow)
        {
            return Result<string>.Expired();
        }

        await _dbContext.ShortLinks.Where(sl => sl.Id == shortLink.Id)
            .ExecuteUpdateAsync(sl => sl.SetProperty(s => s.ClickCount, s => s.ClickCount + 1));
            
        _dbContext.ShortLinkClicks.Add(new ShortLinkClick { ShortLinkId = shortLink.Id });
        await _dbContext.SaveChangesAsync();
        return Result<string>.Ok(shortLink.OriginalUrl);
    }


}