namespace BioWings.Application.Features.Results.GenusResults;
public class GenusSearchQueryResult
{
    public int Id { get; set; }
    public string GenusName { get; set; }
    public int? FamilyId { get; set; }
    public string? FamilyName { get; set; }
}
