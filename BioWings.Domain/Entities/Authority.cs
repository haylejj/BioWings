
namespace BioWings.Domain.Entities;

public class Authority : BaseEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? Year { get; set; }
    public virtual ICollection<Species> Species { get; set; }
}
