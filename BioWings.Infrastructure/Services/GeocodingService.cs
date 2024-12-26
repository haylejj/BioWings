using BioWings.Application.DTOs.NominatimApiDtos;
using BioWings.Application.Services;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.Json;

namespace BioWings.Infrastructure.Services;
public class GeocodingService(ILogger<GeocodingService> logger, IHttpClientFactory httpClientFactory) : IGeocodingService
{
    private Dictionary<string, (int easting, int northing)> _gridOffsets = CreateGridOffsets();
    public int CalculateUTMZone(double longitude) => (int)Math.Floor((longitude + 180) / 6) + 1;

    public (double utmX, double utmY) ConvertMGRSToUTM(double x, double y, string squareRef)
    {
        var gridLetters = squareRef.Substring(0, 2);
        var gridNumbers = squareRef.Substring(2, 2);

        if (!_gridOffsets.TryGetValue(gridLetters, out var offset))
            throw new ArgumentException($"Geçersiz grid referansı: {squareRef}");

        int eastingOffset = int.Parse(gridNumbers[0].ToString()) * 100000;
        int northingOffset = int.Parse(gridNumbers[1].ToString()) * 100000;

        double utmX = x + eastingOffset;
        double utmY = y + northingOffset;

        return (utmX, utmY);
    }
    private static Dictionary<string, (int easting, int northing)> CreateGridOffsets()
    {
        var offsets = new Dictionary<string, (int easting, int northing)>();
        var letters = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        for (int i = 0; i < letters.Length; i++)
            for (int j = 0; j < letters.Length; j++)
            {
                string gridCode = letters[i] + letters[j];
                int easting = (i % 8) + 1;
                int northing = (j % 20) + 1;
                offsets[gridCode] = (easting, northing);
            }

        return offsets;
    }
    public (double latitude, double longitude) ConvertUtmToLatLong(double utmX, double utmY, int utmZone)
    {
        const double k0 = 0.9996;
        const double a = 6378137; // WGS84 ellipsoid yarıçapı
        const double f = 1 / 298.257223563; // WGS84 düzleştirme
        const double b = a * (1 - f);
        double e = Math.Sqrt((Math.Pow(a, 2) - Math.Pow(b, 2)) / Math.Pow(a, 2));
        double e1sq = Math.Pow(e, 2) / (1 - Math.Pow(e, 2));

        // False easting'i kaldır
        utmX -= 500000.0;

        // Yarıküre kontrolü
        bool isNorthernHemisphere = true;
        if (!isNorthernHemisphere) utmY -= 10000000.0;

        double M = utmY / k0;
        double mu = M / (a * (1 - (Math.Pow(e, 2) / 4) - (3 * Math.Pow(e, 4) / 64) - (5 * Math.Pow(e, 6) / 256)));

        double phi1 = mu + (((3 * e1sq / 2) - (27 * Math.Pow(e1sq, 3) / 32)) * Math.Sin(2 * mu))
                        + (((21 * Math.Pow(e1sq, 2) / 16) - (55 * Math.Pow(e1sq, 4) / 32)) * Math.Sin(4 * mu))
                        + (151 * Math.Pow(e1sq, 3) / 96 * Math.Sin(6 * mu));

        double N1 = a / Math.Sqrt(1 - Math.Pow(e * Math.Sin(phi1), 2));
        double T1 = Math.Pow(Math.Tan(phi1), 2);
        double C1 = e1sq * Math.Pow(Math.Cos(phi1), 2);
        double R1 = a * (1 - Math.Pow(e, 2)) / Math.Pow(1 - Math.Pow(e * Math.Sin(phi1), 2), 1.5);
        double D = utmX / (N1 * k0);

        double latitude = phi1 - (N1 * Math.Tan(phi1) / R1 *
                         ((Math.Pow(D, 2) / 2) - ((5 + (3 * T1) + (10 * C1) - (4 * Math.Pow(C1, 2)) - (9 * e1sq)) * Math.Pow(D, 4) / 24) +
                         ((61 + (90 * T1) + (298 * C1) + (45 * Math.Pow(T1, 2)) - (252 * e1sq) - (3 * Math.Pow(C1, 2))) * Math.Pow(D, 6) / 720)));

        double longitude = (D - ((1 + (2 * T1) + C1) * Math.Pow(D, 3) / 6) +
                          ((5 - (2 * C1) + (28 * T1) - (3 * Math.Pow(C1, 2)) + (8 * e1sq) + (24 * Math.Pow(T1, 2))) * Math.Pow(D, 5) / 120)) /
                          Math.Cos(phi1);

        latitude = latitude * 180 / Math.PI;
        longitude = (longitude * 180 / Math.PI) + (((utmZone - 1) * 6) - 180 + 3); // Merkez meridyene göre düzelt

        return (latitude, longitude);
    }

    public async Task<(double latitude, double longitude)> GetLatitudeAndLongitudeByProvinceNameAsync(string provinceName)
    {
        var client = httpClientFactory.CreateClient();
        var url = $"http://localhost:8080/search?q={provinceName},Turkey&format=json";
        try
        {
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<NominatimSearchResponse>>(content);
                logger.LogInformation("Successfully get latitude and longitude from geocoding service");
                return (double.Parse(result[0].Latitude, CultureInfo.InvariantCulture), double.Parse(result[0].Longitude, CultureInfo.InvariantCulture));
            }
            logger.LogError("Failed to get latitude and longitude from geocoding service");
            return (0, 0);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get latitude and longitude from geocoding service");
            return (0, 0);
        }
    }

    public async Task<string> GetProvinceAsync(decimal latitude, decimal longitude)
    {
        var client = httpClientFactory.CreateClient();
        try
        {
            var url = "http://localhost:8080/reverse?format=json&lat=" + latitude.ToString(CultureInfo.InvariantCulture) + "&lon=" + longitude.ToString(CultureInfo.InvariantCulture) + "&accept-language=tr";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<NominatimResponse>(content);
                logger.LogInformation("Successfully get province name from geocoding service");
                var iso = result?.Address?.ISO3166;
                var provinceCode = iso?.Split('-')[1];
                return provinceCode;
            }
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Geocoding Api failed");
            return null;
        }
    }
}
