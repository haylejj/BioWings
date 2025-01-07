using OfficeOpenXml;

namespace BioWings.Application.Helper;
public static class ExcelFormatDetectorHelper
{
    public static string DetectFormat(ExcelWorksheet worksheet)
    {
        var headerRow = 1;
        var headers = GetHeaders(worksheet, headerRow);

        return headers.Contains("Province No:")
            ? "Format1"
            : headers.Contains("Raw Record")
            ? "Format2"
            : headers.Contains("Day 1")
            ? "Format5"
            : headers.Contains("lat") && headers.Contains("lng") ? "Format3" : headers.Contains("Authority Name") ? "Format4" : "Format4";
    }
    public static string DetectFormatForSpeciesImport(ExcelWorksheet worksheet)
    {
        var headerRow = 1;
        var headers = GetHeaders(worksheet, headerRow);
        return headers.Contains("Authority") ? "Format2" : headers.Contains("Hesselbarth Name") ? "Format1" : "Unknown";
    }

    private static HashSet<string> GetHeaders(ExcelWorksheet worksheet, int headerRow)
    {
        var headers = new HashSet<string>();
        var columnCount = worksheet.Dimension.Columns;

        for (int col = 1; col <= columnCount; col++)
        {
            var headerValue = worksheet.Cells[headerRow, col].Text?.Trim();
            if (!string.IsNullOrEmpty(headerValue))
            {
                headers.Add(headerValue);
            }
        }

        return headers;
    }
}
