using OfficeOpenXml;

namespace BioWings.Application.Helper;
public static class ExcelFormatDetectorHelper
{
    public static string DetectFormat(ExcelWorksheet worksheet)
    {
        var headerRow = 1; // Varsayılan header row
        var headers = GetHeaders(worksheet, headerRow);

        // Format1 için characteristic columns
        if (headers.Contains("Province No:"))
            return "Format1";

        // Format2 için characteristic columns
        return headers.Contains("Raw Record")
            ? "Format2"
            : headers.Contains("Day 1")
            ? "Format5"
            : headers.Contains("lat") && headers.Contains("lng") ? "Format3" : headers.Contains("Authority Name") ? "Format4" : "Format4";
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
