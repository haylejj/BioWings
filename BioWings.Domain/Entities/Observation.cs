namespace BioWings.Domain.Entities;

public class Observation : BaseEntity
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Sex { get; set; }
    public DateTime ObservationDate { get; set; }
    public string Method { get; set; }
    public string Activity { get; set; }
    public string LifeStage { get; set; }
    public int NumberSeen { get; set; }
    public decimal Accuracy { get; set; }
    public string Notes { get; set; }
    public string Source { get; set; }
    public string Link { get; set; }
    // Navigation Properties
    public int SpeciesId { get; set; }
    public virtual Species Species { get; set; }
    public int LocationId { get; set; }
    public virtual Location Location { get; set; }
    public int ObserverId { get; set; }
    public virtual Observer Observer { get; set; }
    public virtual ICollection<Media> Media { get; set; }
}
