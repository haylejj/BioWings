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
}
