using BioWings.Domain.Enums;

namespace BioWings.UI.ViewModels.ObservationViewModels;

public class ObservationGetViewModel
{
    public int Id { get; set; }
    //Species
    //Authority
    public string? AuthorityName { get; set; }
    public int? Year { get; set; }
    //Genus
    public string? GenusName { get; set; }
    //Family
    public string? FamilyName { get; set; }
    //SpeciesType
    public string? SpeciesTypeName { get; set; }
    //
    public string? ScientificName { get; set; }
    public string? Name { get; set; }
    public string? EUName { get; set; }
    public string? FullName { get; set; }
    public string? HesselbarthName { get; set; }
    public string? TurkishName { get; set; }
    public string? EnglishName { get; set; }
    public string? TurkishNamesTrakel { get; set; }
    public string? Trakel { get; set; }
    public string? KocakName { get; set; }
    //Location
    //Province
    public string? ProvinceName { get; set; }
    public string? SquareRef { get; set; }
    public decimal SquareLatitude { get; set; }
    public decimal SquareLongitude { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? DecimalDegrees { get; set; }
    public string? DegreesMinutesSeconds { get; set; }
    public string? DecimalMinutes { get; set; }
    public string? UtmCoordinates { get; set; }
    public string? MgrsCoordinates { get; set; }
    public decimal? Altitude1 { get; set; }
    public decimal? Altitude2 { get; set; }
    public string? UtmReference { get; set; }
    public string? Description { get; set; }
    public CoordinatePrecisionLevel CoordinatePrecisionLevel { get; set; }
    //Observer
    public string? ObserverFullName { get; set; }
    public string? Sex { get; set; }
    public DateTime ObservationDate { get; set; }
    public string? LifeStage { get; set; }
    public int? NumberSeen { get; set; }
    public string? Notes { get; set; } //raw records ile notes aynı şeyler.(2 farklı exceldeki sütun)
    public string? Source { get; set; }
    public string? LocationInfo { get; set; }
}
