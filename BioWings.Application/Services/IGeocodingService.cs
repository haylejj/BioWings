namespace BioWings.Application.Services;
public interface IGeocodingService
{
    Task<string> GetProvinceAsync(decimal latitude, decimal longitude);
    Task<(double latitude, double longitude)> GetLatitudeAndLongitudeByProvinceNameAsync(string provinceName);
    int CalculateUTMZone(double longitude);
    (double latitude, double longitude) ConvertUtmToLatLong(double utmX, double utmY, int utmZone);
    public (double utmX, double utmY) ConvertMGRSToUTM(double x, double y, string squareRef);
}
