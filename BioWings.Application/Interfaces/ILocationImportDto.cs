using BioWings.Domain.Enums;

namespace BioWings.Application.Interfaces;
public interface ILocationImportDto
{
    decimal Latitude { get; }
    decimal Longitude { get; }
    string? SquareRef { get; }
    decimal SquareLatitude { get; }
    decimal SquareLongitude { get; }
    string? DecimalDegrees { get; }
    string? DegreesMinutesSeconds { get; }
    string? DecimalMinutes { get; }
    string? UtmCoordinates { get; }
    string? MgrsCoordinates { get; }
    decimal Altitude1 { get; }
    decimal Altitude2 { get; }
    string? UtmReference { get; }
    CoordinatePrecisionLevel CoordinatePrecisionLevel { get; }
}
