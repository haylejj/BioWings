namespace BioWings.Domain.Keys;
public class AuthorityKey
{
    public string Name { get; set; }
    public int Year { get; set; }

    public override bool Equals(object obj)
    {
        return obj is AuthorityKey key &&Name == key.Name && Year == key.Year;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Year);
    }
}
