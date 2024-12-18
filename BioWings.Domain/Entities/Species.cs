namespace BioWings.Domain.Entities;

public class Species : BaseEntity
{
    public int Id { get; set; }
    public int? AuthorityId { get; set; }
    public int? GenusId { get; set; }
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
    public virtual Authority Authority { get; set; }
    public virtual Genus Genus { get; set; }
    public virtual ICollection<Subspecies> Subspecies { get; set; }
    public virtual ICollection<Observation> Observations { get; set; }
}
