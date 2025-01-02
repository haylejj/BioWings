using BioWings.Application.DTOs.StatisticsDtos;

namespace BioWings.Application.Features.Results.StatisticResults;
public class StatisticsGetQueryResult
{
    public int TotalObservationCount { get; set; }
    public int TotalSpeciesCount { get; set; }
    public int TotalFamilyCount { get; set; }
    public int TotalGenusCount { get; set; }
    public int TotalObserverCount { get; set; }
    public List<ProvinceStatDto> TopProvinces { get; set; }
    public List<FamilyStatDto> TopFamilies { get; set; }
    public List<SpeciesStatDto> TopSpecies { get; set; }
}
