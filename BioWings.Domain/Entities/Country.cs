namespace BioWings.Domain.Entities;
public class Country : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<User> Users { get; set; }
}
