using BioWings.Domain.Configuration;
using BioWings.UI.Handler;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BioWings.UI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUiAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.ExpireTimeSpan = TimeSpan.FromHours(12);
                options.SlidingExpiration = true;
                options.LoginPath = "/Login/Login";
                options.LogoutPath = "/Logout/Logout";
                options.AccessDeniedPath = "/Home/AccessDenied";
            });

        return services;
    }

    public static IServiceCollection AddUiHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<TokenHandler>();
        services.AddHttpClient();

        services.AddHttpClient("ApiClient")
           .AddHttpMessageHandler<TokenHandler>();

        // inject ApiSettings from appsettings.json
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));

        return services;
    }
}
