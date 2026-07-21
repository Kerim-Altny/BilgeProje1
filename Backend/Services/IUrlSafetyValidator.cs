namespace Backend.Services;

public sealed record UrlValidationResult(bool IsValid, string? ErrorMessage, string? NormalizedUrl)
{
    public static UrlValidationResult Valid(string normalizedUrl) => new(true, null, normalizedUrl);
    public static UrlValidationResult Invalid(string errorMessage) => new(false, errorMessage, null);
}

public interface IUrlSafetyValidator
{
    // Open redirect / phishing kontrolü. Link oluşturulurken ve yönlendirme anında çağrılır.
    UrlValidationResult Validate(string url);
}
