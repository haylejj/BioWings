using BioWings.Application.DTOs.ExportDtos;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using OfficeOpenXml;

namespace BioWings.Infrastructure.Services;
public class ExcelExportService : IExcelExportService
{
    public byte[] ExportToExcel(IEnumerable<Observation> observations, List<ExpertColumnInfo> columns)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Observations");

        //Column name yazma 
        var columnIndex = 1;
        foreach (var columnn in columns)
        {
            worksheet.Cells[1, columnIndex].Value = columnn.DisplayName;
            worksheet.Cells[1, columnIndex].Style.Font.Bold = true;
            columnIndex++;
        }

        //Data yazma
        var rowIndex = 2;
        foreach (var observation in observations)
        {
            columnIndex = 1;
            foreach (var column in columns)
            {
                var value = GetPropertyValue(observation, column.PropertyPath, column.TableName);

                // Null kontrolü ile değer atama
                worksheet.Cells[rowIndex, columnIndex].Value = value ?? "";
                if (column.PropertyPath.EndsWith("Date") || column.PropertyPath.EndsWith("DateTime"))
                {
                    worksheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "yyyy-mm-dd";
                }
                columnIndex++;
            }
            rowIndex++;
        }

        // Excel ayarları
        worksheet.Cells.AutoFitColumns();

        // Her sütuna stil uygula
        for (int col = 1; col <= columns.Count; col++)
        {
            worksheet.Column(col).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            worksheet.Column(col).AutoFit();
        }

        // En üst satırı dondur
        worksheet.View.FreezePanes(2, 1);

        return package.GetAsByteArray();
    }
    private object GetPropertyValue(Observation observation, string propertyPath, string tableName)
    {
        try
        {
            object currentObject = observation;

            // Property path'i takip et
            var properties = propertyPath.Split('.');
            foreach (var property in properties)
            {
                if (currentObject == null) return null;

                var propertyInfo = currentObject.GetType().GetProperty(property);
                if (propertyInfo == null) return null;

                currentObject = propertyInfo.GetValue(currentObject);
                if (currentObject is DateTime dateValue)
                {
                    return dateValue.ToString("yyyy-MM-dd");
                }
            }

            return currentObject;
        }
        catch (Exception)
        {
            return null;
        }
    }
    public List<ExpertColumnInfo> GetDefaultColumns()
    {
        return new List<ExpertColumnInfo>
        {
            // Authority & Taxonomy
            new() { PropertyPath = "Species.Authority.Name", DisplayName = "Authority Name", TableName = "Species" },
            new() { PropertyPath = "Species.Authority.Year", DisplayName = "Year", TableName = "Species" },
            new() { PropertyPath = "Species.Genus.Family.Name", DisplayName = "Family Name", TableName = "Species" },
            new() { PropertyPath = "Species.Genus.Name", DisplayName = "Genus Name", TableName = "Species" },
            new() { PropertyPath = "Species.Subspecies.Name", DisplayName = "Subspecies Name", TableName = "Subspecies" },
            new() { PropertyPath = "Species.ScientificName", DisplayName = "Scientific Name", TableName = "Species" },
            new() { PropertyPath = "Species.Name", DisplayName = "Name", TableName = "Species" },
            new() { PropertyPath = "Species.EUName", DisplayName = "EU Name", TableName = "Species" },
            new() { PropertyPath = "Species.FullName", DisplayName = "Full Name", TableName = "Species" },
            new() { PropertyPath = "Species.TurkishName", DisplayName = "Turkish Name", TableName = "Species" },
            new() { PropertyPath = "Species.EnglishName", DisplayName = "English Name", TableName = "Species" },
            new() { PropertyPath = "Species.TurkishNamesTrakel", DisplayName = "Turkish Names Trakel", TableName = "Species" },
            new() { PropertyPath = "Species.Trakel", DisplayName = "Trakel", TableName = "Species" },
            new() { PropertyPath = "Species.KocakName", DisplayName = "Kocak Name", TableName = "Species" },
            new() { PropertyPath = "Species.HesselbarthName", DisplayName = "Hesselbarth Name", TableName = "Species" },

            // Location
            new() { PropertyPath = "Location.Province.Name", DisplayName = "Province Name", TableName = "Location" },
            new() { PropertyPath = "Location.Province.ProvinceCode", DisplayName = "Province Code", TableName = "Location" },
            new() { PropertyPath = "Location.SquareRef", DisplayName = "Square Ref", TableName = "Location" },
            new() { PropertyPath = "Location.SquareLatitude", DisplayName = "Square Latitude", TableName = "Location" },
            new() { PropertyPath = "Location.SquareLongitude", DisplayName = "Square Longitude", TableName = "Location" },
            new() { PropertyPath = "Location.Latitude", DisplayName = "Latitude", TableName = "Location" },
            new() { PropertyPath = "Location.Longitude", DisplayName = "Longitude", TableName = "Location" },
            new() { PropertyPath = "Location.DecimalDegrees", DisplayName = "Decimal Degrees", TableName = "Location" },
            new() { PropertyPath = "Location.DegreesMinutesSeconds", DisplayName = "Degrees Minutes Seconds", TableName = "Location" },
            new() { PropertyPath = "Location.DecimalMinutes", DisplayName = "Decimal Minutes", TableName = "Location" },
            new() { PropertyPath = "Location.UtmCoordinates", DisplayName = "UTM Coordinates", TableName = "Location" },
            new() { PropertyPath = "Location.MgrsCoordinates", DisplayName = "MGRS Coordinates", TableName = "Location" },
            new() { PropertyPath = "Location.Altitude1", DisplayName = "Altitude 1", TableName = "Location" },
            new() { PropertyPath = "Location.Altitude2", DisplayName = "Altitude 2", TableName = "Location" },
            new() { PropertyPath = "Location.UtmReference", DisplayName = "UTM Reference", TableName = "Location" },
            new() { PropertyPath = "Location.CoordinatePrecisionLevel", DisplayName = "Coordinate Precision Level", TableName = "Location" },

            // Observer
            new() { PropertyPath = "Observer.Name", DisplayName = "Observer Name", TableName = "Observer" },
            new() { PropertyPath = "Observer.Surname", DisplayName = "Observer Surname", TableName = "Observer" },
            new() { PropertyPath = "Observer.FullName", DisplayName = "Observer Full Name", TableName = "Observer" },

            // Observation Details
            new() { PropertyPath = "Sex", DisplayName = "Sex", TableName = "Observation" },
            new() { PropertyPath = "ObservationDate", DisplayName = "Observation Date", TableName = "Observation" },
            new() { PropertyPath = "LifeStage", DisplayName = "Life Stage", TableName = "Observation" },
            new() { PropertyPath = "NumberSeen", DisplayName = "Number Seen", TableName = "Observation" },
            new() { PropertyPath = "Notes", DisplayName = "Notes", TableName = "Observation" },
            new() { PropertyPath = "Source", DisplayName = "Source", TableName = "Observation" },
            new() { PropertyPath = "LocationInfo", DisplayName = "Location Info", TableName = "Observation" }
        };
    }
}
