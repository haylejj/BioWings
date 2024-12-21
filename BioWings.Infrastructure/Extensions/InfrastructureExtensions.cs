using BioWings.Application.Services;
using BioWings.Infrastructure.Services.ExcelImport;
using BioWings.Infrastructure.Services.ExcelTemplate;
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
        return services;
    }
}
