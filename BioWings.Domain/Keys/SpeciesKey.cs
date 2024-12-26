namespace BioWings.Domain.Keys;

public class SpeciesKey
{
    public string Name { get; set; }
    public int? GenusId { get; set; }
    public int? AuthorityId { get; set; }

    public override bool Equals(object obj)
    {
        return obj is SpeciesKey key &&Name == key.Name && GenusId == key.GenusId && AuthorityId == key.AuthorityId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, GenusId, AuthorityId);
    }
}
