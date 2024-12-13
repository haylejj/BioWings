using BioWings.Application.DTOs.ObservationDtos;
using BioWings.Application.Features.Commands.ObservationCommands;
using BioWings.Application.Results;
using BioWings.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace BioWings.Application.Features.Handlers.ObservationHandlers.Write;
public class ObservationImportCreateCommandHandler(ILogger<ObservationImportCreateCommandHandler> _logger, IMediator _mediator) : IRequestHandler<ObservationImportCreateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(ObservationImportCreateCommand request, CancellationToken cancellationToken)
    {
        if (request.File == null || request.File.Length == 0)
            return ServiceResult.Error("No file uploaded");

        using var package = new ExcelPackage(request.File.OpenReadStream());
        var worksheet = package.Workbook.Worksheets["Import Template"];

        if (worksheet == null)
            return ServiceResult.Error("Invalid template format: 'Import Template' sheet not found");

        var observations = new List<ObservationCreateDto>();
        int rowCount = worksheet.Dimension.Rows;

        // İlk satır header olduğu için 2'den başlıyoruz
        for (int row = 2; row <= rowCount; row++)
        {
            // Boş satırı kontrol et
            if (worksheet.Cells[row, 1].Value == null)
                continue;

            var observation = new ObservationCreateDto
            {
                // Authority & Taxonomy
                AuthorityName = GetCellValue(worksheet, row, 1),
                Year = int.Parse(GetCellValue(worksheet, row, 2) ?? "0"),
                FamilyName = GetCellValue(worksheet, row, 3),
                GenusName = GetCellValue(worksheet, row, 4),
                SubspeciesName = GetCellValue(worksheet, row, 5),
                SpeciesTypeName = GetCellValue(worksheet, row, 6),
                SpeciesTypeDescription = GetCellValue(worksheet, row, 7),
                ScientificName = GetCellValue(worksheet, row, 8),
                Name = GetCellValue(worksheet, row, 9),
                EUName = GetCellValue(worksheet, row, 10),
                FullName = GetCellValue(worksheet, row, 11),
                TurkishName = GetCellValue(worksheet, row, 12),
                EnglishName = GetCellValue(worksheet, row, 13),
                TurkishNamesTrakel = GetCellValue(worksheet, row, 14),
                Trakel = GetCellValue(worksheet, row, 15),
                KocakName = GetCellValue(worksheet, row, 16),
                HesselbarthName = GetCellValue(worksheet, row, 17),

                // Location
                ProvinceName = GetCellValue(worksheet, row, 18),
                ProvinceCode = int.Parse(GetCellValue(worksheet, row, 19) ?? "0"),
                SquareRef = GetCellValue(worksheet, row, 20),
                SquareLatitude = decimal.Parse(GetCellValue(worksheet, row, 21) ?? "0"),
                SquareLongitude = decimal.Parse(GetCellValue(worksheet, row, 22) ?? "0"),
                Latitude = decimal.Parse(GetCellValue(worksheet, row, 23) ?? "0"),
                Longitude = decimal.Parse(GetCellValue(worksheet, row, 24) ?? "0"),
                DecimalDegrees = GetCellValue(worksheet, row, 25),
                DegreesMinutesSeconds = GetCellValue(worksheet, row, 26),
                DecimalMinutes = GetCellValue(worksheet, row, 27),
                UtmCoordinates = GetCellValue(worksheet, row, 28),
                MgrsCoordinates = GetCellValue(worksheet, row, 29),
                Altitude1 = decimal.Parse(GetCellValue(worksheet, row, 30) ?? "0"),
                Altitude2 = decimal.Parse(GetCellValue(worksheet, row, 31) ?? "0"),
                UtmReference = GetCellValue(worksheet, row, 32),
                CoordinatePrecisionLevel = Enum.Parse<CoordinatePrecisionLevel>(GetCellValue(worksheet, row, 33) ?? "Low"),

                // Observer
                ObserverName = GetCellValue(worksheet, row, 34),
                Surname = GetCellValue(worksheet, row, 35),
                ObserverFullName = GetCellValue(worksheet, row, 36),

                // Observation Details
                Sex = GetCellValue(worksheet, row, 37),
                ObservationDate = DateTime.Parse(GetCellValue(worksheet, row, 38) ?? DateTime.Now.ToString()),
                LifeStage = GetCellValue(worksheet, row, 39),
                NumberSeen = int.Parse(GetCellValue(worksheet, row, 40) ?? "0"),
                Notes = GetCellValue(worksheet, row, 41),
                Source = GetCellValue(worksheet, row, 42),
                LocationInfo = GetCellValue(worksheet, row, 43)
            };

            observations.Add(observation);
        }

        if (!observations.Any())
            return ServiceResult.Error("No valid data found in the file");

        _logger.LogInformation("Parsed {Count} observations from Excel file", observations.Count);

        // ObservationCreateRangeCommand'i kullanarak verileri işle
        var rangeCommand = new ObservationCreateRangeCommand { ObservationCreateDtos = observations };
        return await _mediator.Send(rangeCommand, cancellationToken);
    }

    private string? GetCellValue(ExcelWorksheet worksheet, int row, int column)
    {
        var cell = worksheet.Cells[row, column];
        return cell?.Value?.ToString()?.Trim();
    }
}
