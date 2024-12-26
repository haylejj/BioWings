namespace BioWings.Domain.Keys;

public class LocationKey
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public override bool Equals(object obj)
    {
        return obj is LocationKey key &&Latitude == key.Latitude && Longitude == key.Longitude;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Latitude, Longitude);
    }
}
