using Backend.Models;
using Backend.DTOs.Links;

namespace Backend.Services;

public interface IShortLinkService
{
    Task<MyLinksSummaryResponse> GetMyLinksSummaryAsync(int userId);
    Task<IReadOnlyList<ClickChartPointResponse>> GetClickChartAsync(int userId);
    Task<AdminLinksListResponse> GetAdminLinksAsync(string? search, int page, int pageSize);
    Task<Result<ShortLinkResponse>> CreateShortLinkAsync(ShortLinkCreateRequest shortLinkCreateRequest, int userId);
    Task<Result<ShortLinkResponse>> UpdateShortLinkAsync(long shortLinkId, ShortLinkUpdateRequest shortLinkUpdateRequest, int userId, bool canManageAll);
    Task<Result<bool>> DeleteShortLinkAsync(long shortLinkId, int userId, bool canManageAll);
    Task<Result<string>> ResolveAndTrackClickAsync(string shortLinkCode);
}