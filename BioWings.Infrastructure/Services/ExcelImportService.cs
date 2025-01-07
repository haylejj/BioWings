using BioWings.Application.DTOs.ImportDtos;
using BioWings.Application.Helper;
using BioWings.Application.Mappings;
using BioWings.Application.Services;
using BioWings.Domain.Enums;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BioWings.Infrastructure.Services;
public class ExcelImportService : IExcelImportService
{
    private readonly ILogger<ExcelImportService> _logger;
    private readonly Dictionary<string, ExcelMapping> _mappings;
    private readonly Dictionary<string, ExcelMapping> _speciesMappings;
    private readonly IGeocodingService _geocodingService;
    private static readonly Regex _authorityPattern = new(@"\((.*?)(?:,\s*\[?(\d{4})\]?)?\)");
    //ObservationImport
    public ExcelImportService(ILogger<ExcelImportService> logger, IGeocodingService geocodingService)
    {
        _geocodingService=geocodingService;
        _logger = logger;
        _mappings = GetDefaultMappings();
        _speciesMappings = GetDefaultSpeciesMapping();
    }
    public async Task<List<ImportCreateDto>> ImportFromExcelAsync(IFormFile file)
    {
        try
        {
            List<ImportCreateDto> allData = new();

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = file.OpenReadStream();
                using var newPackage = new ExcelPackage();
                var xlsxBytes = ConvertXlsToXlsx(file, stream, newPackage);
                using var convertedPackage = new ExcelPackage(new MemoryStream(xlsxBytes));
                return await ProcessAllSheets(convertedPackage);
            }

            using var originalStream = file.OpenReadStream();
            using var originalPackage = new ExcelPackage(originalStream);
            return await ProcessAllSheets(originalPackage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excel import işlemi sırasında hata oluştu");
            throw;
        }
    }
    private async Task<List<ImportCreateDto>> ProcessAllSheets(ExcelPackage package)
    {
        List<ImportCreateDto> allData = new();

        foreach (var worksheet in package.Workbook.Worksheets)
        {
            if (worksheet?.Dimension == null) continue;

            string format = ExcelFormatDetectorHelper.DetectFormat(worksheet);
            _logger.LogInformation($"Excel formatı: {format}");
            if (!_mappings.ContainsKey(format)) continue;

            var mapping = _mappings[format];
            var columnMappings = CreateColumnMappings(worksheet, mapping);
            var sheetData = await ProcessRows(worksheet, columnMappings, mapping);
            allData.AddRange(sheetData);
        }

        return !allData.Any() ? throw new InvalidOperationException("Excel dosyası boş veya geçersiz format içeriyor.") : allData;
    }
    private Dictionary<string, int> CreateColumnMappings(ExcelWorksheet worksheet, ExcelMapping mapping)
    {
        var columnMappings = new Dictionary<string, int>();
        var headerRow = 1;

        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
        {
            var headerValue = worksheet.Cells[headerRow, col].Text?.Trim();
            if (string.IsNullOrEmpty(headerValue)) continue;

            foreach (var map in mapping.ColumnMappings)
            {
                if (map.Value.Any(possibleHeader =>
                    possibleHeader.Equals(headerValue, StringComparison.OrdinalIgnoreCase)))
                {
                    columnMappings[map.Key] = col;
                    break;
                }
            }
        }

        return columnMappings;
    }
    private async Task<List<ImportCreateDto>> ProcessRows(ExcelWorksheet worksheet, Dictionary<string, int> columnMappings, ExcelMapping mapping)
    {
        var result = new List<ImportCreateDto>();
        var rowCount = worksheet.Dimension.Rows;

        for (int row = mapping.DataStartRow; row <= rowCount; row++)
        {
            try
            {
                var dto = await CreateImportDto(worksheet, row, columnMappings);
                if (dto != null)
                {
                    result.Add(dto);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Satır {row} işlenirken hata oluştu");
                // Hatalı satırı atlayıp devam et
                continue;
            }
        }

        return result;
    }
    private byte[] ConvertXlsToXlsx(IFormFile file, Stream stream, ExcelPackage excelPackage)
    {
        _logger.LogInformation("XLS dosyası XLSX formatına dönüştürülüyor...");

        using var reader = ExcelReaderFactory.CreateReader(stream);
        var result = reader.AsDataSet();

        _logger.LogInformation($"Toplam sheet sayısı: {result.Tables.Count}");

        // Tüm tabloları döngüyle gezip her biri için yeni sheet oluştur
        for (int tableIndex = 0; tableIndex < result.Tables.Count; tableIndex++)
        {
            var table = result.Tables[tableIndex];

            // Sheet adını koru veya varsayılan ad ata
            string sheetName = !string.IsNullOrWhiteSpace(table.TableName)
                ? table.TableName
                : $"Sheet{tableIndex + 1}";

            var worksheet = excelPackage.Workbook.Worksheets.Add(sheetName);

            _logger.LogInformation($"'{sheetName}' sheet'i işleniyor. Satır sayısı: {table.Rows.Count}, Sütun sayısı: {table.Columns.Count}");

            // Tablodaki tüm hücreleri kopyala
            for (int row = 0; row < table.Rows.Count; row++)
            {
                for (int col = 0; col < table.Columns.Count; col++)
                {
                    var cellValue = table.Rows[row][col];
                    if (cellValue != null)
                    {
                        worksheet.Cells[row + 1, col + 1].Value = cellValue;

                        // Tarih formatını koru
                        if (cellValue is DateTime)
                        {
                            worksheet.Cells[row + 1, col + 1].Style.Numberformat.Format = "dd.mm.yyyy";
                        }
                    }
                }
            }

            // Sütun genişliklerini otomatik ayarla
            worksheet.Cells.AutoFitColumns();
        }

        _logger.LogInformation($"Dönüştürme tamamlandı. XLSX dosyasında {excelPackage.Workbook.Worksheets.Count} sheet bulunuyor");

        return excelPackage.GetAsByteArray();
    }
    private async Task<ImportCreateDto> CreateImportDto(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMappings)
    {
        var dto = new ImportCreateDto
        {
            // Taxonomy
            AuthorityName = GetCellValue(worksheet, row, columnMappings, "AuthorityName"),
            AuthorityYear = GetIntValue(worksheet, row, columnMappings, "AuthorityYear"),
            GenusName = GetCellValue(worksheet, row, columnMappings, "GenusName"),
            FamilyName = GetCellValue(worksheet, row, columnMappings, "FamilyName"),
            ScientificName = GetCellValue(worksheet, row, columnMappings, "ScientificName"),
            SpeciesName = GetCellValue(worksheet, row, columnMappings, "SpeciesName"),
            FullName = GetCellValue(worksheet, row, columnMappings, "FullName"),
            TurkishName = GetCellValue(worksheet, row, columnMappings, "TurkishName"),
            EnglishName = GetCellValue(worksheet, row, columnMappings, "EnglishName"),
            SubspeciesName = GetCellValue(worksheet, row, columnMappings, "SubspeciesName"),

            // Location
            ProvinceName = GetCellValue(worksheet, row, columnMappings, "ProvinceName"),
            ProvinceCode = GetNullableIntValue(worksheet, row, columnMappings, "ProvinceCode"),
            SquareRef = GetCellValue(worksheet, row, columnMappings, "SquareRef"),
            X= GetDecimalValue(worksheet, row, columnMappings, "X"),
            Y= GetDecimalValue(worksheet, row, columnMappings, "Y"),
            Latitude = GetDecimalValue(worksheet, row, columnMappings, "Latitude"),
            Longitude = GetDecimalValue(worksheet, row, columnMappings, "Longitude"),
            LocationInfo = GetCellValue(worksheet, row, columnMappings, "LocationInfo"),
            Altitude1 = GetDecimalValue(worksheet, row, columnMappings, "Altitude1"),
            Altitude2 = GetDecimalValue(worksheet, row, columnMappings, "Altitude2"),

            // Observer
            ObserverName = GetCellValue(worksheet, row, columnMappings, "ObserverName"),
            ObserverFullName = GetCellValue(worksheet, row, columnMappings, "ObserverFullName"),
            ObserverSurname= GetCellValue(worksheet, row, columnMappings, "ObserverSurname"),
            // Observation Details
            Sex = GetCellValue(worksheet, row, columnMappings, "Sex"),
            ObservationDate = DateTimeParserHelper.ParseObservationDate(worksheet, row, columnMappings),
            LifeStage = GetCellValue(worksheet, row, columnMappings, "LifeStage"),
            NumberSeen = GetIntValue(worksheet, row, columnMappings, "NumberSeen", 1),
            Notes = GetCellValue(worksheet, row, columnMappings, "Notes"),
            Source = GetCellValue(worksheet, row, columnMappings, "Source"),

            TurkishNamesTrakel = GetCellValue(worksheet, row, columnMappings, "TurkishNamesTrakel"),
            Trakel = GetCellValue(worksheet, row, columnMappings, "Trakel"),
            KocakName = GetCellValue(worksheet, row, columnMappings, "KocakName"),
            HesselbarthName = GetCellValue(worksheet, row, columnMappings, "HesselbarthName"),
            EUName = GetCellValue(worksheet, row, columnMappings, "EUName"),
            SquareLatitude = GetDecimalValue(worksheet, row, columnMappings, "SquareLatitude"),
            SquareLongitude = GetDecimalValue(worksheet, row, columnMappings, "SquareLongitude"),
            DecimalDegrees = GetCellValue(worksheet, row, columnMappings, "DecimalDegrees"),
            DegreesMinutesSeconds = GetCellValue(worksheet, row, columnMappings, "DegreesMinutesSeconds"),
            DecimalMinutes = GetCellValue(worksheet, row, columnMappings, "DecimalMinutes"),
            UtmCoordinates = GetCellValue(worksheet, row, columnMappings, "UtmCoordinates"),
            MgrsCoordinates = GetCellValue(worksheet, row, columnMappings, "MgrsCoordinates"),
            UtmReference = GetCellValue(worksheet, row, columnMappings, "UtmReference"),
            CoordinatePrecisionLevel = GetEnumValue(worksheet, row, columnMappings, "CoordinatePrecisionLevel")
        };
        //if (dto.ProvinceName == null && dto.Latitude != 0 && dto.Longitude != 0)
        //{
        //    var provinceCode = await _geocodingService.GetProvinceAsync(dto.Latitude, dto.Longitude);
        //    if (provinceCode == null)
        //    {
        //        _logger.LogWarning($"Province bulunamadı: Lat={dto.Latitude}, Lon={dto.Longitude}");
        //    }
        //    else
        //    {
        //        dto.ProvinceCode = int.Parse(provinceCode);
        //    }

        //}
        if (dto.X!=0 && dto.Y!=0 && dto.SquareRef != null)
        {
            dto.Latitude = 10;
            dto.Longitude = 10;
        }
        return dto;
    }
    //Common methods
    private CoordinatePrecisionLevel GetEnumValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMappings, string propertyName)
    {
        if (columnMappings.TryGetValue(propertyName, out int column))
        {
            var cell = worksheet.Cells[row, column];

            if (cell.Value is double numericValue)
            {
                return (CoordinatePrecisionLevel)Convert.ToInt32(numericValue);
            }

            var stringValue = cell.Text?.Trim();
            if (!string.IsNullOrEmpty(stringValue))
            {
                // 2.1 String içinde sayı olabilir ("0", "1", "2" gibi)
                if (int.TryParse(stringValue, out int intValue))
                {
                    return (CoordinatePrecisionLevel)intValue;
                }

                // Enum isim olarak gelmiş olabilir ("TamHassasKoordinat", "UTMKoordinati" gibi)
                if (Enum.TryParse<CoordinatePrecisionLevel>(stringValue, true, out var result))
                {
                    return result;
                }
            }
        }

        return CoordinatePrecisionLevel.ExactCoordinate;
    }
    private string GetCellValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMappings, string propertyName)
    {
        if (columnMappings.TryGetValue(propertyName, out int column))
        {
            var value = worksheet.Cells[row, column].Text?.Trim();
            return string.IsNullOrWhiteSpace(value) ? null : value;
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
                return propertyName.Contains("Latitude")
                    ? Math.Max(-90, Math.Min(90, result))
                    : propertyName.Contains("Longitude") ? Math.Max(-180, Math.Min(180, result)) : result;
            }
        }
        return defaultValue;
    }
    //SpeciesImport
    private static Dictionary<string, ExcelMapping> GetDefaultMappings()
    {
        return new Dictionary<string, ExcelMapping>
    {
        {
                //DKM data All data_Mar2014_Ayse
            "Format1", new ExcelMapping
            {
                SheetName = "Sheet1",
                ColumnMappings = new Dictionary<string, string[]>
                {
                    // Taxonomy
                    { "GenusName", new[] { "Genus" } },
                    { "SpeciesName", new[] { "Species" } },
                    { "FullName", new[] { "Full name" } },
                    
                    // Location
                    { "ProvinceCode", new[] { "Province No:" } },
                    { "ProvinceName", new[] { "Province" } },
                    { "SquareRef", new[] { "Square" } },
                    { "Latitude", new[] { "X" } },
                    { "Longitude", new[] { "Y" } },
                    { "LocationInfo", new[] { "Location" } },
                    
                    // Observer & Source
                    { "ObserverName", new[] { "Observer" } },
                    { "Source", new[] { "Source" } },
                    
                    // Date
                    { "Day", new[] { "Day" } },
                    { "Month", new[] { "Month" } },
                    { "Year", new[] { "Year" } }
                }
            }
        },
        {
                //Hesselbarth et al 1995  data
            "Format2", new ExcelMapping
            {
                SheetName = "Sheet1",
                ColumnMappings = new Dictionary<string, string[]>
                {
                    // Taxonomy
                    { "GenusName", new[] { "Genus" } },
                    { "SpeciesName", new[] { "Species" } },
                    { "FullName", new[] { "Full name" } },
                    { "SubspeciesName", new[] { "Subspecies" } },
                    
                    // Location
                    { "ProvinceCode", new[] { "Prov. No." } },
                    { "ProvinceName", new[] { "Prov. Name" } },
                    { "SquareRef", new[] { "Sq. Ref" } },
                    { "Latitude", new[] { "Enlem" } },
                    { "Longitude", new[] { "Boylam" } },
                    { "LocationInfo", new[] { "Loc. Info" } },
                    { "Altitude1", new[] { "Altitude" } },
                    { "Altitude2", new[] { "Altitude2" } },
                    { "UtmReference", new[] { "UTM_10X10" } },
                    
                    // Other
                    { "Notes", new[] { "Raw Record" } },
                    { "Source", new[] { "Source" } },
                    
                    // Date
                    { "Day", new[] { "Day" } },
                    { "Month", new[] { "Month" } },
                    { "Year", new[] { "Year" } }
                }
            }
        },
        {
                //Tablib Dataset observations-dkm-vakfi_2023
            "Format3", new ExcelMapping
            {
                SheetName = "Sheet1",
                ColumnMappings = new Dictionary<string, string[]>
                {
                    // Taxonomy
                    { "ScientificName", new[] { "scientific name" } },
                    { "SpeciesName", new[] { "species name" } },
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
                    
                    // Date & Time
                    { "date", new[] { "date" } },
                    { "time", new[] { "time" } }
                }
            }
        },
        {
                //Default template
            "Format4", new ExcelMapping
            {
                SheetName = "Sheet1",
                ColumnMappings = new Dictionary<string, string[]>
                {
                    // Authority
                    { "AuthorityName", new[] { "Authority Name" } },
                    { "AuthorityYear", new[] { "Year" } },
                     
                    // Taxonomy
                    { "GenusName", new[] { "Genus Name" } },
                    { "FamilyName", new[] { "Family Name" } },
                    { "ScientificName", new[] { "Scientific Name" } },
                    { "SpeciesName", new[] { "Name" } },
                    { "EUName", new[] { "EU Name" } },
                    { "FullName", new[] { "Full Name" } },
                    { "TurkishName", new[] { "Turkish Name" } },
                    { "EnglishName", new[] { "English Name" } },
                    { "TurkishNamesTrakel", new[] { "Turkish Names Trakel" } },
                    { "Trakel", new[] { "Trakel" } },
                    { "KocakName", new[] { "Kocak Name" } },
                    { "HesselbarthName", new[] { "Hesselbarth Name" } },
                    { "SubspeciesName", new[] { "Subspecies Name" } },
                    
                    // Location
                    { "ProvinceName", new[] { "Province Name" } },
                    { "ProvinceCode", new[] { "Province Code" } },
                    { "SquareRef", new[] { "Square Ref" } },
                    { "SquareLatitude", new[] { "Square Latitude" } },
                    { "SquareLongitude", new[] { "Square Longitude" } },
                    { "Latitude", new[] { "Latitude" } },
                    { "Longitude", new[] { "Longitude" } },
                    { "DecimalDegrees", new[] { "Decimal Degrees" } },
                    { "DegreesMinutesSeconds", new[] { "Degrees Minutes Seconds" } },
                    { "DecimalMinutes", new[] { "Decimal Minutes" } },
                    { "UtmCoordinates", new[] { "UTM Coordinates" } },
                    { "MgrsCoordinates", new[] { "MGRS Coordinates" } },
                    { "Altitude1", new[] { "Altitude 1" } },
                    { "Altitude2", new[] { "Altitude 2" } },
                    { "UtmReference", new[] { "UTM Reference" } },
                    
                    // Observer
                    { "ObserverName", new[] { "Observer Name" } },
                    { "ObserverSurname", new[] { "Observer Surname" } },
                    { "ObserverFullName", new[] { "Observer Full Name" } },
                    
                    // Others
                    { "Sex", new[] { "Sex" } },
                    { "ObservationDate", new[] { "Observation Date" } },
                    { "LifeStage", new[] { "Life Stage" } },
                    { "NumberSeen", new[] { "Number Seen" } },
                    { "Notes", new[] { "Notes" } },
                    { "Source", new[] { "Source" } },
                    { "LocationInfo", new[] { "Location Info" } }
                }
            }
        },
        {
                //Mixed obs since 1995 All data_Mar2014_Ayse
            "Format5", new ExcelMapping
            {
               SheetName = "Sheet1",
                ColumnMappings = new Dictionary<string, string[]>
                {
                    // Taxonomy
                    { "GenusName", new[] { "Genus" } },
                    { "SpeciesName", new[] { "Species" } },
                    { "FullName", new[] { "Full name" } },
                    { "SubspeciesName", new[] { "Subspecies" } },
                    
                    // Location
                    { "ProvinceCode", new[] { "Prov. No." } },
                    { "ProvinceName", new[] { "Prov. Name" } },
                    { "SquareRef", new[] { "Square" } },
                    { "X", new[] { "X" } },
                    { "Y", new[] { "Y" } },
                    { "LocationInfo", new[] { "Location"} },
                    { "Altitude1", new[] { "Altitude 1" } },
                    { "Altitude2", new[] { "Altitude 2" } },
                    { "UtmReference", new[] { "UTM_10X10" } },
                    
                    // Other
                    { "Source", new[] { "Source" } },
                    
                    // Date
                    { "Day", new[] { "Day 1" } },
                    { "Month", new[] { "Month 1" } },
                    { "Year", new[] { "Year" } }
                }
            }
        },
    };
    }
    private static Dictionary<string, ExcelMapping> GetDefaultSpeciesMapping()
    {
        return new Dictionary<string, ExcelMapping>
        {
            { "Format1",new ExcelMapping
                {
                    SheetName = "Sheet1",
                    ColumnMappings=new Dictionary<string, string[]>
                    {
                        { "AuthorityName", new [] {"Authority Name" } },
                        { "AuthorityYear", new [] {"Authority Year" } },
                        { "GenusName", new [] {"Genus Name" }},
                        { "FamilyName",new [] { "Family Name" }},
                        { "ScientificName",new [] { "Scientific Name" }},
                        { "SpeciesName", new [] {"Species Name" }},
                        { "EUName", new [] {"EU Name" }},
                        { "FullName",new [] { "Full Name" }},
                        { "TurkishName", new [] {"Turkish Name" }},
                        { "EnglishName", new [] {"English Name" }},
                        { "TurkishNamesTrakel",new [] { "Turkish Names Trakel" }},
                        { "Trakel",new [] { "Trakel" }},
                        { "KocakName", new [] {"Kocak Name" }},
                        { "HesselbarthName",new [] { "Hesselbarth Name" }}
                    }
                }
            },
            { "Format2",new ExcelMapping
                {
                    SheetName = "Sheet1",
                    ColumnMappings=new Dictionary<string, string[]>
                    {
                        { "EUName", new [] { "Species EU" } },
                        { "AuthorityName", new [] {"Authority" } },
                        { "FamilyName",new [] { "Family" }},
                        { "SpeciesName", new [] {"English Name" }},
                        { "Turkish Name", new [] {"Turkish Name" }},
                        { "EnglishName", new [] {"English Name" }},
                        { "TurkishNamesTrakel",new [] { "Turkish Names Trakel" }},
                        { "Trakel",new [] { "Species Trakel" }},
                    }
                }
            }
        };
    }
    private (string? Name, int? Year) ParseAuthority(string? authorityText)
    {
        if (string.IsNullOrWhiteSpace(authorityText))
            return (null, null);

        var match = _authorityPattern.Match(authorityText);

        if (!match.Success)
            return (authorityText?.Trim(), null);

        var name = match.Groups[1].Value.Trim();
        var yearStr = match.Groups[2].Value;

        return int.TryParse(yearStr, out int year) ? ((string? Name, int? Year))(name, year) : ((string? Name, int? Year))(name, null);
    }
    private ImportCreateSpeciesDto CreateSpeciesDto(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMappings)
    {
        var dto = new ImportCreateSpeciesDto
        {
            AuthorityName = GetCellValue(worksheet, row, columnMappings, "AuthorityName"),
            AuthorityYear = GetNullableIntValue(worksheet, row, columnMappings, "AuthorityYear"),
            GenusName = GetCellValue(worksheet, row, columnMappings, "GenusName"),
            FamilyName = GetCellValue(worksheet, row, columnMappings, "FamilyName"),
            ScientificName = GetCellValue(worksheet, row, columnMappings, "ScientificName"),
            SpeciesName = GetCellValue(worksheet, row, columnMappings, "SpeciesName"),
            EUName = GetCellValue(worksheet, row, columnMappings, "EUName"),
            FullName = GetCellValue(worksheet, row, columnMappings, "FullName"),
            TurkishName = GetCellValue(worksheet, row, columnMappings, "TurkishName"),
            EnglishName = GetCellValue(worksheet, row, columnMappings, "EnglishName"),
            TurkishNamesTrakel = GetCellValue(worksheet, row, columnMappings, "TurkishNamesTrakel"),
            Trakel = GetCellValue(worksheet, row, columnMappings, "Trakel"),
            KocakName = GetCellValue(worksheet, row, columnMappings, "KocakName"),
            HesselbarthName = GetCellValue(worksheet, row, columnMappings, "HesselbarthName")
        };
        if (dto.SpeciesName == null) dto.SpeciesName=dto.EnglishName;

        // Authority bilgisini parse et
        if (columnMappings.TryGetValue("AuthorityName", out int authorityCol))
        {
            var authorityText = worksheet.Cells[row, authorityCol].Text;
            if (!string.IsNullOrEmpty(authorityText))
            {
                var (name, year) = ParseAuthority(authorityText);
                if (dto.AuthorityName == null) dto.AuthorityName = name;
                if (dto.AuthorityYear == null) dto.AuthorityYear = year;
            }
        }

        return dto;
    }
    private List<ImportCreateSpeciesDto> ProcessSpeciesRows(ExcelWorksheet worksheet, Dictionary<string, int> columnMappings)
    {
        var result = new List<ImportCreateSpeciesDto>();
        var rowCount = worksheet.Dimension.Rows;

        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                var dto = CreateSpeciesDto(worksheet, row, columnMappings);
                if (dto != null && !string.IsNullOrWhiteSpace(dto.SpeciesName))
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
    private List<ImportCreateSpeciesDto> ProcessAllSheetsForSpecies(ExcelPackage package)
    {
        List<ImportCreateSpeciesDto> allData = new();
        var targetSheet = package.Workbook.Worksheets[0];

        if (targetSheet?.Dimension == null)
            throw new InvalidOperationException("Hedef sheet bulunamadı veya boş.");

        string format = ExcelFormatDetectorHelper.DetectFormatForSpeciesImport(targetSheet);
        _logger.LogInformation($"Excel formatı: {format}");

        if (!_speciesMappings.ContainsKey(format))
            throw new InvalidOperationException("Geçersiz Excel formatı.");

        var mapping = _speciesMappings[format];
        var columnMappings = CreateSpeciesColumnMappings(targetSheet, mapping);
        allData.AddRange(ProcessSpeciesRows(targetSheet, columnMappings));

        return !allData.Any()
            ? throw new InvalidOperationException("Excel dosyası boş veya geçersiz format içeriyor.")
            : allData;
    }
    public List<ImportCreateSpeciesDto> ImportSpeciesFromExcel(IFormFile file)
    {
        try
        {
            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Sadece XLSX formatında dosyalar kabul edilmektedir.");
            }
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Dosya boş veya geçersiz.");
            }
            using var originalStream = file.OpenReadStream();
            using var originalPackage = new ExcelPackage(originalStream);
            return ProcessAllSheetsForSpecies(originalPackage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excel import işlemi sırasında hata oluştu");
            throw;
        }
    }
    private Dictionary<string, int> CreateSpeciesColumnMappings(ExcelWorksheet worksheet, ExcelMapping mapping)
    {
        var columnMappings = new Dictionary<string, int>();
        var headerRow = 1;

        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
        {
            var headerValue = worksheet.Cells[headerRow, col].Text?.Trim();
            if (string.IsNullOrEmpty(headerValue)) continue;

            foreach (var map in mapping.ColumnMappings)
            {
                if (map.Value.Any(possibleHeader =>
                    possibleHeader.Equals(headerValue, StringComparison.OrdinalIgnoreCase)))
                {
                    columnMappings[map.Key] = col;
                    break;
                }
            }
        }

        return columnMappings;
    }
}

