namespace Backend.DTOs.Links;

public class MyLinksSummaryResponse
{
    public int TotalLinks { get; set; }
    public long TotalClicks { get; set; }
    public string? TopLink { get; set; }
    public IReadOnlyList<ShortLinkResponse> Links { get; set; } = [];
}
