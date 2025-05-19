namespace BioWings.Domain.Entities;

public class Observer : BaseEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? FullName { get; set; }
    public virtual ICollection<Observation>? Observations { get; set; }
}