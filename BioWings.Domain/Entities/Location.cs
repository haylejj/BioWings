using BioWings.Domain.Enums;

namespace BioWings.Domain.Entities;

public class Location : BaseEntity
{
    public int Id { get; set; }
    public int ProvinceId { get; set; }
    public string? SquareRef { get; set; }
    public decimal SquareLatitude { get; set; }
    public decimal SquareLongitude { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? DecimalDegrees { get; set; }        // "41.015137, 28.979530"
    public string? DegreesMinutesSeconds { get; set; } // "41°00'54.5"N 28°58'46.3"E"
    public string? DecimalMinutes { get; set; }        // "41°0.908'N 28°57.772'E"
    public string? UtmCoordinates { get; set; }        // "35T 683453 4543121"
    public string? MgrsCoordinates { get; set; }       // "35T CG 83453 43121"
    public decimal Altitude1 { get; set; }
    public decimal Altitude2 { get; set; }
    public string? UtmReference { get; set; }
    public string? Description { get; set; }
    public CoordinatePrecisionLevel CoordinatePrecisionLevel { get; set; }
    public virtual Province Province { get; set; }
    public virtual ICollection<Observation> Observations { get; set; }
}
