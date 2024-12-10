using System.ComponentModel.DataAnnotations;

namespace BioWings.Domain.Enums;
public enum CoordinatePrecisionLevel
{
    [Display(Name = "Tam/Hassas Koordinat")]
    ExactCoordinate = 0,    // Tam/hassas koordinat (GPS ile alınmış)
    [Display(Name = "UTM Koordinatı")]
    UTMCoordinate = 1,      // UTM grid sistemi koordinatı
    [Display(Name = "İlçe Koordinatı")]
    CountyCoordinate = 2,   // İlçe seviyesi koordinat
    [Display(Name = "Kare Koordinatı")]
    SquareCoordinate = 3,   // Grid kare referansı
    [Display(Name = "İl Koordinatı")]
    ProvinceCoordinate = 4  // İl seviyesi koordinat
}
