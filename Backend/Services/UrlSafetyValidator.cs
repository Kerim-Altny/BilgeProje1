using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Sockets;

namespace Backend.Services;

public class UrlSafetyValidator : IUrlSafetyValidator
{
    private readonly string[] _ownDomains;

    public UrlSafetyValidator(IConfiguration configuration)
    {
        _ownDomains = configuration.GetSection("UrlSafety:OwnDomains").Get<string[]>() ?? Array.Empty<string>();
    }

    public UrlValidationResult Validate(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return UrlValidationResult.Invalid("Geçerli bir URL değil.");
        }

        if (uri.Scheme != "http" && uri.Scheme != "https")
        {
            return UrlValidationResult.Invalid("Sadece http/https şemalarına izin verilir.");
        }

        if (url.Length > 2048)
        {
            return UrlValidationResult.Invalid("URL çok uzun.");
        }

        if (!string.IsNullOrEmpty(uri.UserInfo))
        {
            return UrlValidationResult.Invalid("URL kullanıcı bilgisi içeremez.");
        }

        if (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) || IsLoopbackOrPrivate(uri.Host))
        {
            return UrlValidationResult.Invalid("Yerel veya özel ağ adreslerine izin verilmez.");
        }

        if (_ownDomains.Any(domain => uri.Host.Equals(domain, StringComparison.OrdinalIgnoreCase)))
        {
            return UrlValidationResult.Invalid("URL kendi domainimize yönlendiremez.");
        }

        return UrlValidationResult.Valid(uri.ToString());
    }

    private static bool IsLoopbackOrPrivate(string host)
    {
        if (!IPAddress.TryParse(host, out var ip))
        {
            return false;
        }

        if (IPAddress.IsLoopback(ip))
        {
            return true;
        }

        if (ip.AddressFamily != AddressFamily.InterNetwork)
        {
            return false;
        }

        var bytes = ip.GetAddressBytes();
        return bytes[0] switch
        {
            10 => true,
            127 => true,
            169 => bytes[1] == 254,
            192 => bytes[1] == 168,
            _ => false
        };
    }
}
