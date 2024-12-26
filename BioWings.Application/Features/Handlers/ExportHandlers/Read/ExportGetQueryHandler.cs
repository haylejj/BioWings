using BioWings.Application.Features.Queries.ExportQueries;
using BioWings.Application.Features.Results.ExportResults;
using BioWings.Application.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.ExportHandlers.Read;
public class ExportGetQueryHandler(ILogger<ExportGetQueryHandler> logger) : IRequestHandler<ExportGetQuery, ServiceResult<IEnumerable<ExportGetQueryResult>>>
{
    public Task<ServiceResult<IEnumerable<ExportGetQueryResult>>> Handle(ExportGetQuery request, CancellationToken cancellationToken)
    {
        var columns = new List<ExportGetQueryResult>
    {
        // Authority related columns
        new() {
            PropertyPath = "Species.Authority.Name",
            DisplayName = "Authority Name",
            TableName = "Species",
            DataType = "string"
        },
        new() {
            PropertyPath = "Species.Authority.Year",
            DisplayName = "Authority Year",
            TableName = "Species",
            DataType = "int"
        },

        // Taxonomy related columns (Family, Genus, Species)
        new() {
            PropertyPath = "Species.Genus.Family.Name",
            DisplayName = "Family Name",
            TableName = "Species",
            DataType = "string"
        },
        new() {
            PropertyPath = "Species.Genus.Name",
            DisplayName = "Genus Name",
            TableName = "Species",
            DataType = "string"
        },
        new() {
            PropertyPath = "Species.ScientificName",
            DisplayName = "Scientific Name",
            TableName = "Species",
            DataType = "string"
        },
        new() {
            PropertyPath = "Species.Name",
            DisplayName = "Species Name",
            TableName = "Species",
            DataType = "string"
        },
        new() {
            PropertyPath = "Species.EUName",
            DisplayName = "EU Name",
            TableName = "Species",
            DataType = "string"
        },
        new() {
            PropertyPath = "Species.FullName",
            DisplayName = "Full Name",
            TableName = "Species",
            DataType = "string"
        },
        new() {
            PropertyPath = "Species.TurkishName",
            DisplayName = "Turkish Name",
            TableName = "Species",
            DataType = "string"
        },
        new() {
            PropertyPath = "Species.EnglishName",
            DisplayName = "English Name",
            TableName = "Species",
            DataType = "string"
        },
        new() {
            PropertyPath = "Species.TurkishNamesTrakel",
            DisplayName = "Turkish Names (Trakel)",
            TableName = "Species",
            DataType = "string"
        },
        new() {
            PropertyPath = "Species.Trakel",
            DisplayName = "Trakel",
            TableName = "Species",
            DataType = "string"
        },
        new() {
            PropertyPath = "Species.KocakName",
            DisplayName = "Kocak Name",
            TableName = "Species",
            DataType = "string"
        },
        new() {
            PropertyPath = "Species.HesselbarthName",
            DisplayName = "Hesselbarth Name",
            TableName = "Species",
            DataType = "string"
        },
        // Location related columns
        new() {
            PropertyPath = "Location.Province.Name",
            DisplayName = "Province Name",
            TableName = "Location",
            DataType = "string"
        },
        new() {
            PropertyPath = "Location.Province.ProvinceCode",
            DisplayName = "Province Code",
            TableName = "Location",
            DataType = "int"
        },
        new() {
            PropertyPath = "Location.SquareRef",
            DisplayName = "Square Reference",
            TableName = "Location",
            DataType = "string"
        },
        new() {
            PropertyPath = "Location.SquareLatitude",
            DisplayName = "Square Latitude",
            TableName = "Location",
            DataType = "decimal"
        },
        new() {
            PropertyPath = "Location.SquareLongitude",
            DisplayName = "Square Longitude",
            TableName = "Location",
            DataType = "decimal"
        },
        new() {
            PropertyPath = "Location.Latitude",
            DisplayName = "Latitude",
            TableName = "Location",
            DataType = "decimal"
        },
        new() {
            PropertyPath = "Location.Longitude",
            DisplayName = "Longitude",
            TableName = "Location",
            DataType = "decimal"
        },
        new() {
            PropertyPath = "Location.DecimalDegrees",
            DisplayName = "Decimal Degrees",
            TableName = "Location",
            DataType = "string"
        },
        new() {
            PropertyPath = "Location.DegreesMinutesSeconds",
            DisplayName = "Degrees Minutes Seconds",
            TableName = "Location",
            DataType = "string"
        },
        new() {
            PropertyPath = "Location.DecimalMinutes",
            DisplayName = "Decimal Minutes",
            TableName = "Location",
            DataType = "string"
        },
        new() {
            PropertyPath = "Location.UtmCoordinates",
            DisplayName = "UTM Coordinates",
            TableName = "Location",
            DataType = "string"
        },
        new() {
            PropertyPath = "Location.MgrsCoordinates",
            DisplayName = "MGRS Coordinates",
            TableName = "Location",
            DataType = "string"
        },
        new() {
            PropertyPath = "Location.Altitude1",
            DisplayName = "Altitude 1",
            TableName = "Location",
            DataType = "decimal"
        },
        new() {
            PropertyPath = "Location.Altitude2",
            DisplayName = "Altitude 2",
            TableName = "Location",
            DataType = "decimal"
        },
        new() {
            PropertyPath = "Location.UtmReference",
            DisplayName = "UTM Reference",
            TableName = "Location",
            DataType = "string"
        },
        new() {
            PropertyPath = "Location.CoordinatePrecisionLevel",
            DisplayName = "Coordinate Precision Level",
            TableName = "Location",
            DataType = "enum"
        },

        // Observer related columns
        new() {
            PropertyPath = "Observer.Name",
            DisplayName = "Observer Name",
            TableName = "Observer",
            DataType = "string"
        },
        new() {
            PropertyPath = "Observer.Surname",
            DisplayName = "Observer Surname",
            TableName = "Observer",
            DataType = "string"
        },
        new() {
            PropertyPath = "Observer.FullName",
            DisplayName = "Observer Full Name",
            TableName = "Observer",
            DataType = "string"
        },

        // Observation details
        new() {
            PropertyPath = "Sex",
            DisplayName = "Sex",
            TableName = "Observation",
            DataType = "string"
        },
        new() {
            PropertyPath = "ObservationDate",
            DisplayName = "Observation Date",
            TableName = "Observation",
            DataType = "datetime"
        },
        new() {
            PropertyPath = "LifeStage",
            DisplayName = "Life Stage",
            TableName = "Observation",
            DataType = "string"
        },
        new() {
            PropertyPath = "NumberSeen",
            DisplayName = "Number Seen",
            TableName = "Observation",
            DataType = "int"
        },
        new() {
            PropertyPath = "Notes",
            DisplayName = "Notes",
            TableName = "Observation",
            DataType = "string"
        },
        new() {
            PropertyPath = "Source",
            DisplayName = "Source",
            TableName = "Observation",
            DataType = "string"
        },
        new() {
            PropertyPath = "LocationInfo",
            DisplayName = "Location Info",
            TableName = "Observation",
            DataType = "string"
        }
    };

        logger.LogInformation("Export columns are generated successfully.");
        return Task.FromResult(ServiceResult<IEnumerable<ExportGetQueryResult>>.Success(columns));
    }
}
