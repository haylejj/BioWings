using BioWings.Domain.Enums;

namespace BioWings.Application.Features.Results.LocationResults;
public class LocationGetQueryResult
{
    public int Id { get; set; }
    public int? ProvinceId { get; set; }
    public string ProvinceName { get; set; }
    public string SquareRef { get; set; }
    public decimal SquareLatitude { get; set; }
    public decimal SquareLongitude { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? DecimalDegrees { get; set; }
    public string? DegreesMinutesSeconds { get; set; }
    public string? DecimalMinutes { get; set; }
    public string? UtmCoordinates { get; set; }
    public string? MgrsCoordinates { get; set; }
    public decimal Altitude1 { get; set; }
    public decimal Altitude2 { get; set; }
    public string UtmReference { get; set; }
    public string Description { get; set; }
    public CoordinatePrecisionLevel CoordinatePrecisionLevel { get; set; }
}
