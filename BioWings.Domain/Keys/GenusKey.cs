namespace BioWings.Domain.Keys;

public class GenusKey
{
    public string? GenusName { get; set; }
    public int? FamilyId { get; set; }

    public override bool Equals(object obj)
    {
        return obj is GenusKey key &&GenusName == key.GenusName && FamilyId == key.FamilyId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GenusName, FamilyId);
    }
}
