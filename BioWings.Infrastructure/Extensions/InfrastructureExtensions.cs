using BioWings.Application.Services;
using BioWings.Infrastructure.Authentication.Services;
using BioWings.Infrastructure.Services;
using BioWings.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;

namespace BioWings.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        services.AddScoped<IExcelTemplateService, ExcelTemplateService>();
        services.AddScoped<IExcelImportService, ExcelImportService>();
        services.AddScoped<IExcelExportService, ExcelExportService>();
        services.AddScoped<IGeocodingService, GeocodingService>();
        services.AddScoped<IObservationImportProgressTracker, ObservationImportProgressTracker>();
        services.AddScoped<IProgressTracker, ProgressTracker>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHashService, PasswordHashService>();

        // Email Service - MailHog kullanılıyor
        services.AddScoped<IEmailService, MailHogService>();
        services.Configure<MailHogSettings>(configuration.GetSection("MailHog"));
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.Configure<EncryptionSettings>(configuration.GetSection("EncryptionSettings"));
        services.Configure<NominatimSettings>(configuration.GetSection("NominatimSettings"));
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<ILoginLogService, LoginLogService>();
        services.AddScoped<IIpAddressService, IpAddressService>();

        // Redis Configuration
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:ConnectionString"];
            options.InstanceName = "BioWings_";
        });
        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}
