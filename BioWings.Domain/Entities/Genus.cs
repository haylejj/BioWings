namespace BioWings.Domain.Entities;

public class Genus : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    //Navigation Properties
    public int FamilyId { get; set; }
    public Family Family { get; set; }
    public virtual ICollection<Species> Species { get; set; }
}
