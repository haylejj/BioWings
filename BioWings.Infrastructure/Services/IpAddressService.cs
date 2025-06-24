using Microsoft.AspNetCore.Http;

namespace BioWings.Infrastructure.Services;

public interface IIpAddressService
{
    string GetClientIpAddress(HttpContext httpContext);
}

public class IpAddressService : IIpAddressService
{
    public string GetClientIpAddress(HttpContext httpContext)
    {
        // X-Forwarded-For header'ını kontrol et (proxy arkasındaysa)
        var xForwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xForwardedFor))
        {
            // Birden fazla IP varsa ilkini al (gerçek client IP)
            var ips = xForwardedFor.Split(',');
            if (ips.Length > 0)
            {
                return ips[0].Trim();
            }
        }

        // X-Real-IP header'ını kontrol et
        var xRealIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xRealIp))
        {
            return xRealIp.Trim();
        }

        // CF-Connecting-IP header'ını kontrol et (Cloudflare için)
        var cfConnectingIp = httpContext.Request.Headers["CF-Connecting-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(cfConnectingIp))
        {
            return cfConnectingIp.Trim();
        }

        // Remote IP adresini kullan
        var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString();
        if (!string.IsNullOrEmpty(remoteIp))
        {
            // IPv6 localhost'u IPv4'e çevir
            if (remoteIp == "::1")
            {
                return "127.0.0.1";
            }
            return remoteIp;
        }

        return "Unknown";
    }
} 