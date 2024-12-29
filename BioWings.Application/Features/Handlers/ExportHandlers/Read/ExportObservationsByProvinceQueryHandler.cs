using BioWings.Application.Features.Queries.ExportQueries;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ExportHandlers.Read;
public class ExportObservationsByProvinceQueryHandler(IObservationRepository observationRepository, ILogger<ExportObservationsByProvinceQueryHandler> logger, IExcelExportService excelExportService) : IRequestHandler<ExportObservationsByProvinceQuery, ServiceResult<byte[]>>
{
    public async Task<ServiceResult<byte[]>> Handle(ExportObservationsByProvinceQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Starting export process in Export Observations By Province Query Handler");
            var observations = observationRepository.GetObservationsByProvinceIdForExporting(request.ProvinceId);
            var datas = await observations.ToListAsync(cancellationToken);
            if (datas.Count == 0)
            {
                logger.LogWarning("No data found for the given criteria to export");
                return ServiceResult<byte[]>.Error("No data found for the given criteria to export", System.Net.HttpStatusCode.NotFound);
            }
            var excelFile = excelExportService.ExportToExcel(observations, excelExportService.GetDefaultColumns());
            logger.LogInformation("Export process completed in Export Observations By Province Query Handler");
            return ServiceResult<byte[]>.Success(excelFile);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred in Export Observations By Province Query Handler");
            return ServiceResult<byte[]>.Error("Error occurred in Export Observations By Province Query Handler");
        }
    }
}
