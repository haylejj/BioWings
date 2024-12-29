namespace BioWings.Application.Features.Results.ObservationMapResults;
public class ObservationMapGetQueryResult
{
    public int Id { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int ProvinceCode { get; set; }
    public string ProvinceName { get; set; }
}
