namespace BioWings.Domain.Entities;

public class SpeciesType : BaseEntity // belki enum da olabilir ?
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
