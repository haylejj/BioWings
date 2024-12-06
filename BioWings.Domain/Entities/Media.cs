namespace BioWings.Domain.Entities;

public class Media : BaseEntity
{
    public int Id { get; set; }
    public string MediaType { get; set; }
    public string FilePath { get; set; }
    public string Description { get; set; }
    // Navigation Properties
    public int ObservationId { get; set; }
    public virtual Observation Observation { get; set; }
    public int SpeciesId { get; set; }
    public virtual Species Species { get; set; }

}
