namespace BioWings.Domain.Entities;
public class Family : BaseEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    //Navigation Property
    public virtual ICollection<Genus> Genera { get; set; }
}