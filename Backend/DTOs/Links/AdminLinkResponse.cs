namespace Backend.DTOs.Links;

public class AdminLinkResponse
{
    public string Id { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public long ClickCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string Username { get; set; } = string.Empty;
    public int UserId { get; set; }
}

public class AdminLinksListResponse
{
    public int TotalLinks { get; set; }
    public long TotalClicks { get; set; }
    public int TotalUsers { get; set; }
    public IReadOnlyList<AdminLinkResponse> Links { get; set; } = [];
}
