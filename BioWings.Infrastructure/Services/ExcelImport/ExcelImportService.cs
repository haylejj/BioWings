using BioWings.Application.DTOs.ImportDtos;
using BioWings.Application.DTOs.ObservationDtos;
using BioWings.Application.Helper;
using BioWings.Application.Mappings;
using BioWings.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.Globalization;

namespace BioWings.Infrastructure.Services.ExcelImport;
public class ExcelImportService : IExcelImportService
{
    private readonly ILogger<ExcelImportService> _logger;
    private readonly Dictionary<string, ExcelMapping> _mappings;

    public ExcelImportService(ILogger<ExcelImportService> logger)
    {
        _logger = logger;
        _mappings = GetDefaultMappings();
    }
    public List<ImportCreateDto> ImportFromExcel(IFormFile file)
    {
        try
        {
            using var stream = file.OpenReadStream();
            using var package = new ExcelPackage(stream);
            var worksheet = GetWorksheet(package);

            var headerRow = GetHeaderRow(worksheet);
            var (mapping, format) = DetectExcelFormat(headerRow);

            if (mapping == null)
                throw new InvalidOperationException("Excel formatı tanınamadı.");

            var columnMappings = CreateColumnMappings(headerRow, mapping);
            return ProcessRows(worksheet, columnMappings, mapping);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excel import işlemi sırasında hata oluştu");
            throw;
        }
    }

    private ExcelWorksheet GetWorksheet(ExcelPackage package)
    {
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        if (worksheet == null || worksheet.Dimension == null)
            throw new InvalidOperationException("Excel dosyası boş veya geçersiz.");

        return worksheet;
    }

    private Dictionary<string, string> GetHeaderRow(ExcelWorksheet worksheet)
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var colCount = worksheet.Dimension.Columns;

        for (int col = 1; col <= colCount; col++)
        {
            var headerValue = worksheet.Cells[1, col].Text?.Trim();
            if (!string.IsNullOrEmpty(headerValue))
            {
                headers[col.ToString()] = headerValue;
            }
        }

        return headers;
    }

    private (ExcelMapping mapping, string format) DetectExcelFormat(Dictionary<string, string> headers)
    {
        foreach (var formatMapping in _mappings)
        {
            var matchCount = 0;
            var totalColumns = formatMapping.Value.ColumnMappings.Count;
            var requiredColumns = formatMapping.Value.ColumnMappings
                .SelectMany(x => x.Value)
                .ToList();

            foreach (var header in headers.Values)
            {
                if (requiredColumns.Any(rc => rc.Equals(header, StringComparison.OrdinalIgnoreCase)))
                {
                    matchCount++;
                }
            }

            // En az %60 eşleşme olmalı
            if ((double)matchCount / totalColumns >= 0.6)
            {
                return (formatMapping.Value, formatMapping.Key);
            }
        }

        return (null, null);
    }

    private Dictionary<string, int> CreateColumnMappings(Dictionary<string, string> headers, ExcelMapping mapping)
    {
        var columnMappings = new Dictionary<string, int>();

        foreach (var map in mapping.ColumnMappings)
        {
            var propertyName = map.Key;
            var possibleHeaders = map.Value;

            var matchingHeader = headers.FirstOrDefault(h =>
                possibleHeaders.Any(ph => ph.Equals(h.Value, StringComparison.OrdinalIgnoreCase)));

            if (!string.IsNullOrEmpty(matchingHeader.Key))
            {
                columnMappings[propertyName] = int.Parse(matchingHeader.Key);
            }
        }

        return columnMappings;
    }

    private List<ImportCreateDto> ProcessRows(ExcelWorksheet worksheet, Dictionary<string, int> columnMappings, ExcelMapping mapping)
    {
        var result = new List<ImportCreateDto>();
        var rowCount = worksheet.Dimension.Rows;

        for (int row = mapping.DataStartRow; row <= rowCount; row++)
        {
            try
            {
                var dto = CreateDto(worksheet, row, columnMappings);
                if (dto != null)
                {
                    result.Add(dto);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Satır {row} işlenirken hata oluştu");
                continue;
            }
        }

        return result;
    }

    private ImportCreateDto CreateDto(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMappings)
    {
        return new ImportCreateDto
        {
            // Taxonomy Information
            AuthorityName = GetCellValue(worksheet, row, columnMappings, "AuthorityName"),
            AuthorityYear = GetIntValue(worksheet, row, columnMappings, "AuthorityYear"),
            GenusName = GetCellValue(worksheet, row, columnMappings, "GenusName"),
            FamilyName = GetCellValue(worksheet, row, columnMappings, "FamilyName"),
            ScientificName = GetCellValue(worksheet, row, columnMappings, "ScientificName"),
            Name = GetCellValue(worksheet, row, columnMappings, "Name"),
            EUName = GetCellValue(worksheet, row, columnMappings, "EUName"),
            FullName = GetCellValue(worksheet, row, columnMappings, "FullName"),
            TurkishName = GetCellValue(worksheet, row, columnMappings, "TurkishName"),
            EnglishName = GetCellValue(worksheet, row, columnMappings, "EnglishName"),
            TurkishNamesTrakel = GetCellValue(worksheet, row, columnMappings, "TurkishNamesTrakel"),
            Trakel = GetCellValue(worksheet, row, columnMappings, "Trakel"),
            KocakName = GetCellValue(worksheet, row, columnMappings, "KocakName"),
            HesselbarthName = GetCellValue(worksheet, row, columnMappings, "HesselbarthName"),
            SubspeciesName = GetCellValue(worksheet, row, columnMappings, "SubspeciesName"),

            // Location Information
            ProvinceName = GetCellValue(worksheet, row, columnMappings, "ProvinceName"),
            ProvinceCode = GetNullableIntValue(worksheet, row, columnMappings, "ProvinceCode"),
            SquareRef = GetCellValue(worksheet, row, columnMappings, "SquareRef"),
            SquareLatitude = GetDecimalValue(worksheet, row, columnMappings, "SquareLatitude"),
            SquareLongitude = GetDecimalValue(worksheet, row, columnMappings, "SquareLongitude"),
            Latitude = GetDecimalValue(worksheet, row, columnMappings, "Latitude"),
            Longitude = GetDecimalValue(worksheet, row, columnMappings, "Longitude"),
            DecimalDegrees = GetCellValue(worksheet, row, columnMappings, "DecimalDegrees"),
            DegreesMinutesSeconds = GetCellValue(worksheet, row, columnMappings, "DegreesMinutesSeconds"),
            DecimalMinutes = GetCellValue(worksheet, row, columnMappings, "DecimalMinutes"),
            UtmCoordinates = GetCellValue(worksheet, row, columnMappings, "UtmCoordinates"),
            MgrsCoordinates = GetCellValue(worksheet, row, columnMappings, "MgrsCoordinates"),
            Altitude1 = GetDecimalValue(worksheet, row, columnMappings, "Altitude1"),
            Altitude2 = GetDecimalValue(worksheet, row, columnMappings, "Altitude2"),
            UtmReference = GetCellValue(worksheet, row, columnMappings, "UtmReference"),
            Description = GetCellValue(worksheet, row, columnMappings, "Description"),

            // Observer Information
            ObserverName = GetCellValue(worksheet, row, columnMappings, "ObserverName"),
            Surname = GetCellValue(worksheet, row, columnMappings, "Surname"),
            ObserverFullName = GetCellValue(worksheet, row, columnMappings, "ObserverFullName"),

            // Observation Details
            Sex = GetCellValue(worksheet, row, columnMappings, "Sex"),
            ObservationDate = DateTimeParserHelper.ParseObservationDate(worksheet, row, columnMappings),
            LifeStage = GetCellValue(worksheet, row, columnMappings, "LifeStage"),
            NumberSeen = GetIntValue(worksheet, row, columnMappings, "NumberSeen", 1),
            Notes = GetCellValue(worksheet, row, columnMappings, "Notes"),
            Source = GetCellValue(worksheet, row, columnMappings, "Source"),
            LocationInfo = GetCellValue(worksheet, row, columnMappings, "LocationInfo")
        };
    }

    private string GetCellValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMappings, string propertyName)
    {
        if (columnMappings.TryGetValue(propertyName, out int column))
        {
            return worksheet.Cells[row, column].Text?.Trim();
        }
        return null;
    }

    private int GetIntValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMappings, string propertyName, int defaultValue = 0)
    {
        if (columnMappings.TryGetValue(propertyName, out int column))
        {
            if (int.TryParse(worksheet.Cells[row, column].Text, out int value))
            {
                return value;
            }
        }
        return defaultValue;
    }

    private int? GetNullableIntValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMappings, string propertyName)
    {
        if (columnMappings.TryGetValue(propertyName, out int column))
        {
            if (int.TryParse(worksheet.Cells[row, column].Text, out int value))
            {
                return value;
            }
        }
        return null;
    }

    private decimal GetDecimalValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMappings, string propertyName, decimal defaultValue = 0)
    {
        if (columnMappings.TryGetValue(propertyName, out int column))
        {
            var value = worksheet.Cells[row, column].Text?.Replace(',', '.');
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }
        }
        return defaultValue;
    }
    private static Dictionary<string, ExcelMapping> GetDefaultMappings()
    {
        return new Dictionary<string, ExcelMapping>
        {
            {
                "Format1", new ExcelMapping // İlk Excel formatı
                {
                    SheetName = "Sheet1",
                    ColumnMappings = new Dictionary<string, string[]>
                    {
                        // Taxonomy
                        { "GenusName", new[] { "Genus" } },
                        { "Name", new[] { "Species" } },
                        { "FullName", new[] { "Full name" } },
                        
                        // Location
                        { "ProvinceCode", new[] { "Province No." } },
                        { "ProvinceName", new[] { "Province" } },
                        { "SquareRef", new[] { "Square" } },
                        { "Latitude", new[] { "Y" } },
                        { "Longitude", new[] { "X" } },
                        { "LocationInfo", new[] { "Location", "Location1" } },
                        
                        // Observer & Source
                        { "ObserverName", new[] { "Observer" } },
                        { "Source", new[] { "Source" } },
                        
                        // Date (özel işlem gerekiyor)
                        { "Day", new[] { "Day" } },
                        { "Month", new[] { "Month" } },
                        { "Year", new[] { "Year" } }
                    }
                }
            },
            {
                "Format2", new ExcelMapping // İkinci Excel formatı
                {
                    SheetName = "Sheet1",
                    ColumnMappings = new Dictionary<string, string[]>
                    {
                        // Taxonomy
                        { "GenusName", new[] { "Genus" } },
                        { "Name", new[] { "Species" } },
                        { "FullName", new[] { "Full name" } },
                        { "SubspeciesName", new[] { "Subspecies" } },
                        
                        // Location
                        { "ProvinceCode", new[] { "Prov No." } },
                        { "ProvinceName", new[] { "Prov. Name" } },
                        { "SquareRef", new[] { "Sq" } },
                        { "Latitude", new[] { "Y" } },
                        { "Longitude", new[] { "X" } },
                        { "LocationInfo", new[] { "Loc. Info" } },
                        { "Altitude1", new[] { "Altitude" } },
                        
                        // Other
                        { "Notes", new[] { "Raw Record" } },
                        { "Source", new[] { "Source" } },
                        { "Year", new[] { "Year" } }
                    }
                }
            },
            {
                "Format3", new ExcelMapping // Üçüncü Excel formatı
                {
                    SheetName = "Sheet1",
                    ColumnMappings = new Dictionary<string, string[]>
                    {
                        // Taxonomy
                        { "ScientificName", new[] { "scientific name" } },
                        { "Name", new[] { "species name" } },
                        { "FamilyName", new[] { "family" } },
                        
                        // Location
                        { "Latitude", new[] { "lat" } },
                        { "Longitude", new[] { "lng" } },
                        { "LocationInfo", new[] { "location" } },
                        
                        // Observation Details
                        { "Sex", new[] { "sex" } },
                        { "LifeStage", new[] { "life stage" } },
                        { "NumberSeen", new[] { "number" } },
                        { "Notes", new[] { "notes" } },
                        { "Source", new[] { "source" } },
                        
                        // Date & Time (özel işlem gerekiyor)
                        { "ObservationDate", new[] { "date" } },
                        { "ObservationTime", new[] { "time" } }
                    }
                }
            }
        };
    }
}
}
