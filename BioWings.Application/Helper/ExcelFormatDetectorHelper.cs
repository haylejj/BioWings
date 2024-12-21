using OfficeOpenXml;

namespace BioWings.Application.Helper;
public static class ExcelFormatDetectorHelper
{
    public static string DetectFormat(ExcelWorksheet worksheet)
    {
        var headerRow = 1; // Varsayılan header row
        var headers = GetHeaders(worksheet, headerRow);

        // Format1 için characteristic columns
        if (headers.Contains("Province No.") &&
            headers.Contains("Square") &&
            headers.Contains("Day") &&
            headers.Contains("Month") &&
            headers.Contains("Year"))
        {
            return "Format1";
        }

        // Format2 için characteristic columns
        if (headers.Contains("Prov. No.") &&
            headers.Contains("Prov. Name") &&
            headers.Contains("Sq. Ref") &&
            headers.Contains("Raw Record"))
        {
            return "Format2";
        }

        // Format3 için characteristic columns
        return headers.Contains("scientific name") &&
            headers.Contains("life stage") &&
            headers.Contains("date") &&
            headers.Contains("time")
            ? "Format3"
            : headers.Contains("Authority Name") && headers.Contains("Turkish Name")
            ? "Format4"
            : throw new InvalidOperationException("Unknown Excel format");
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
