using BioWings.Application.Features.Commands.ExportCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ExportHandlers.Write;
public class ExportCreateCommandHandler(ILogger<ExportCreateCommandHandler> logger, IObservationRepository observationRepository, IExcelExportService excelExportService) : IRequestHandler<ExportCreateCommand, ServiceResult<byte[]>>
{
    public async Task<ServiceResult<byte[]>> Handle(ExportCreateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Starting export process in Export Create Command Handler");
            var observations = observationRepository.GetObservationsForExporting(request.StartDate, request.EndDate, request.RecordLimit, request.ExportAllDates, request.ExportAllRecords, request.Columns);
            var datas = await observations.ToListAsync(cancellationToken);
            if (datas.Count == 0)
            {
                logger.LogWarning("No data found for the given criteria to export");
                return ServiceResult<byte[]>.Error("No data found for the given criteria to export");
            }
            var excelFile = excelExportService.ExportToExcel(observations, request.Columns);
            logger.LogInformation("Export process completed in Export Create Command Handler");
            return ServiceResult<byte[]>.Success(excelFile);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred in Export Create Command Handler");
            return ServiceResult<byte[]>.Error("Error occurred in Export Create Command Handler");
        }
    }
}
