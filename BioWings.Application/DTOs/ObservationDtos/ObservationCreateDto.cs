using BioWings.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace BioWings.Application.DTOs.ObservationDtos;
public class ObservationCreateDto
{
    public int SpeciesId { get; set; }
    //Species 
    public int AuthorityId { get; set; }
    //Authority
    public string? AuthorityName { get; set; }
    public int Year { get; set; }
    //
    public int GenusId { get; set; }
    //Genus
    public string? GenusName { get; set; }
    //Family
    public int FamilyId { get; set; }
    //
    public string? FamilyName { get; set; }
    //
    public int SpeciesTypeId { get; set; }
    //SpeciesType
    public string? SpeciesTypeName { get; set; }
    public string? SpeciesTypeDescription { get; set; }
    public string? ScientificName { get; set; }
    public string? Name { get; set; }
    public string? EUName { get; set; }
    public string? FullName { get; set; }
    public string? TurkishName { get; set; }
    public string? EnglishName { get; set; }
    public string? TurkishNamesTrakel { get; set; }
    public string? Trakel { get; set; }
    public string? KocakName { get; set; }
    public string? HesselbarthName { get; set; }
    public List<IFormFile>? FormFiles { get; set; } = new List<IFormFile>();//IFormFile is used to upload files . its equivalent to Media 
    //
    public int LocationId { get; set; }
    //Location
    public int ProvinceId { get; set; }
    //Province
    public string? ProvinceName { get; set; }
    public int ProvinceCode { get; set; }
    //
    public string? LocationName { get; set; }
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
    public decimal Altitude1 { get; set; }
    public decimal Altitude2 { get; set; }
    public string? UtmReference { get; set; }
    public string? Description { get; set; }
    public CoordinatePrecisionLevel CoordinatePrecisionLevel { get; set; }
    //
    public int ObserverId { get; set; }
    //Observer
    public string? ObserverName { get; set; }
    public string? Surname { get; set; }
    public string? ObserverFullName { get; set; }
    //
    public string? Sex { get; set; }
    public DateTime ObservationDate { get; set; }
    public string? LifeStage { get; set; }
    public int NumberSeen { get; set; }
    public string? Notes { get; set; } //raw records ile notes aynı şeyler.(2 farklı exceldeki sütun)
    public string? Source { get; set; }
    public string? LocationInfo { get; set; }
}
