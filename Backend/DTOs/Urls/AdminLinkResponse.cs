namespace Backend.DTOs;

public class AdminLinkResponse
{
    public long Id { get; set; }
    public string TargetUrl { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
    public long ClickCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string Username { get; set; } = string.Empty;
    public int UserId { get; set; }
}

public class AdminLinksStatsResponse
{
    public int TotalLinks { get; set; }
    public long TotalClicks { get; set; }
    public int TotalUsers { get; set; }
    public List<AdminLinkResponse> Links { get; set; } = [];
}
