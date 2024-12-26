using System.Text.Json.Serialization;

namespace BioWings.Application.DTOs.NominatimApiDtos;

public class NominatimAddress
{
    [JsonPropertyName("neighbourhood")]
    public string Neighbourhood { get; set; }

    [JsonPropertyName("town")]
    public string Town { get; set; }

    [JsonPropertyName("province")]
    public string Province { get; set; }

    [JsonPropertyName("ISO3166-2-lvl4")]
    public string ISO3166 { get; set; }

    [JsonPropertyName("region")]
    public string Region { get; set; }

    [JsonPropertyName("postcode")]
    public string PostCode { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("country_code")]
    public string CountryCode { get; set; }
}
