namespace BioWings.Domain.Entities;

public class Observation : BaseEntity
{
    public int Id { get; set; }
    public int SpeciesId { get; set; }
    public int LocationId { get; set; }
    public int ObserverId { get; set; }
    public string Sex { get; set; }
    public DateTime ObservationDate { get; set; }
    public string LifeStage { get; set; }
    public int NumberSeen { get; set; }
    public string Notes { get; set; } //raw records ile notes aynı şeyler.(2 farklı exceldeki sütun)
    public string Source { get; set; }
    public string LocationInfo { get; set; }
    // Navigation Properties
    public virtual Species Species { get; set; }
    public virtual Location Location { get; set; }
    public virtual Observer Observer { get; set; }
}
