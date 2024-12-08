namespace BioWings.Application.Features.Results.SpeciesResults;
public class SpeciesGetQueryResult
{
    public int Id { get; set; }
    public int AuthorityId { get; set; }
    public int GenusId { get; set; }
    public int SpeciesTypeId { get; set; }
    public string ScientificName { get; set; }
    public string Name { get; set; }
    public string EUName { get; set; }
    public string FullName { get; set; }
    public string TurkishName { get; set; }
    public string EnglishName { get; set; }
    public string TurkishNamesTrakel { get; set; }
    public string Trakel { get; set; }
}
