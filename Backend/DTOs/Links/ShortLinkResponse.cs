namespace Backend.DTOs.Links;

public class ShortLinkResponse
{
    public string Id { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public long ClickCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpirationDate { get; set; }
}