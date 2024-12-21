using OfficeOpenXml;

namespace BioWings.Application.Helper;
public static class DateTimeParserHelper
{
    public static DateTime ParseObservationDate(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMappings)
    {

        // date ve time sütunları varsa
        if (columnMappings.ContainsKey("date"))
        {
            var dateStr = worksheet.Cells[row, columnMappings["date"]].Text?.Trim();
            var timeStr = columnMappings.ContainsKey("time")
                ? worksheet.Cells[row, columnMappings["time"]].Text?.Trim()
                : null;

            if (!string.IsNullOrEmpty(dateStr))
            {
                var date = DateTime.Parse(dateStr);

                if (!string.IsNullOrEmpty(timeStr))
                {
                    if (TimeSpan.TryParse(timeStr, out var timeSpan))
                    {
                        date = date.Add(timeSpan);
                    }
                }

                return date;
            }
        }

        // Day, Month, Year sütunları varsa
        if (columnMappings.ContainsKey("Day") &&
            columnMappings.ContainsKey("Month") &&
            columnMappings.ContainsKey("Year"))
        {
            var dayStr = worksheet.Cells[row, columnMappings["Day"]].Text?.Trim();
            var monthStr = worksheet.Cells[row, columnMappings["Month"]].Text?.Trim();
            var yearStr = worksheet.Cells[row, columnMappings["Year"]].Text?.Trim();

            if (!string.IsNullOrEmpty(dayStr) &&
                !string.IsNullOrEmpty(monthStr) &&
                !string.IsNullOrEmpty(yearStr))
            {
                var day = int.Parse(dayStr);
                var month = int.Parse(monthStr);
                var year = int.Parse(yearStr);

                return new DateTime(year, month, day);
            }
        }
        // Observation Date sütunu varsa
        if (columnMappings.ContainsKey("ObservationDate"))
        {
            var cell = worksheet.Cells[row, columnMappings["ObservationDate"]];

            // Eğer hücre değeri DateTime ise direkt kullan
            if (cell.Value is DateTime dateValue)
                return dateValue;

            var dateStr = cell.Text?.Trim();
            if (!string.IsNullOrEmpty(dateStr) && DateTime.TryParse(dateStr, out var parsedDate))
            {
                return parsedDate;
            }
        }
        return DateTime.MinValue;
    }
}
