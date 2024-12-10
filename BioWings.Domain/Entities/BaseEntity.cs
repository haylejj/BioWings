namespace BioWings.Domain.Entities;
public abstract class BaseEntity
{
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}
