namespace BioWings.Application.Features.Results.ObservationMapResults;
public class ObservationMapGetByObservationIdQueryResult
{
    public int Id { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string ProvinceName { get; set; }
    public DateTime ObservationDate { get; set; }
    public int NumberSeen { get; set; }
    public string Notes { get; set; }
    public string SpeciesName { get; set; }
    public string ScientificName { get; set; }
    public string KocakName { get; set; }
    public string HesselbartName { get; set; }
    public string GenusName { get; set; }
    public string FamilyName { get; set; }
    public string AuthortyName { get; set; }
    public int? AuthorityYear { get; set; }
}
