namespace BioWings.Domain.Entities;

public class Subspecies : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    // Navigation Properties
    public int SpeciesId { get; set; }
    public virtual Species Species { get; set; }
}
