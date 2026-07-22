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
                return Result<ShortLinkResponse>.Conflict("Custom code already exists.");
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
    public async Task<IReadOnlyList<ShortLinkResponse>> GetAllShortLinksAsync(int userId, bool canManageAll)
    {
        if (!canManageAll)
        {
            return (await _dbContext.ShortLinks.Where(sl => sl.CreatedByUserId == userId).OrderByDescending(sl => sl.CreatedAt).ToListAsync()).Select(_mapper.Map<ShortLinkResponse>).ToList();
        }

        return (await _dbContext.ShortLinks.OrderByDescending(sl => sl.CreatedAt).ToListAsync()).Select(_mapper.Map<ShortLinkResponse>).ToList();
    }

    public async Task<ShortLinkResponse?> GetShortLinkByIdAsync(int userId, long shortLinkId, bool canManageAll)
    {
        var shortLink = await _dbContext.ShortLinks.FirstOrDefaultAsync(sl => sl.Id == shortLinkId);
        if (shortLink is null) return null;
        if (!canManageAll && shortLink!.CreatedByUserId != userId) return null;
        return _mapper.Map<ShortLinkResponse>(shortLink);
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



    public async Task<string?> ResolveAndTrackClickAsync(string shortLinkCode)
    {
        var shortLink = await _dbContext.ShortLinks.FirstOrDefaultAsync(sl => sl.ShortCode == shortLinkCode);
        if (shortLink is null) return null;

        if(shortLink.ExpirationDate.HasValue && shortLink.ExpirationDate.Value < DateTime.UtcNow)
        {
            return null;
        }

        await _dbContext.ShortLinks.Where(sl => sl.Id == shortLink.Id)
            .ExecuteUpdateAsync(sl => sl.SetProperty(s => s.ClickCount, s => s.ClickCount + 1));
        return shortLink.OriginalUrl;
    }


}