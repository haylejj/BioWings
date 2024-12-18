namespace BioWings.Domain.Entities;

public class Province : BaseEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int ProvinceCode { get; set; }
    public virtual ICollection<Location> Locations { get; set; }
}
