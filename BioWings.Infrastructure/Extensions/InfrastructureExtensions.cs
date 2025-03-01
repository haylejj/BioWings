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
        ExcelPackage.LicenseContext =LicenseContext.NonCommercial;
        services.AddScoped<IExcelTemplateService, ExcelTemplateService>();
        services.AddScoped<IExcelImportService, ExcelImportService>();
        services.AddScoped<IExcelExportService, ExcelExportService>();
        services.AddScoped<IGeocodingService, GeocodingService>();
        services.AddScoped<IObservationImportProgressTracker, ObservationImportProgressTracker>();
        services.AddScoped<IProgressTracker, ProgressTracker>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHashService, PasswordHashService>();
        services.AddScoped<IEmailService, EmailService>();
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.Configure<EncryptionSettings>(configuration.GetSection("EncryptionSettings"));
        services.AddScoped<ILoginService, LoginService>();
        return services;
    }
}
