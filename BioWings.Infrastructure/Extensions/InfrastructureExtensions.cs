using BioWings.Application.Services;
using BioWings.Infrastructure.Services;
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
        return services;
    }
}
