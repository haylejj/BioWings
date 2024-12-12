using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace BioWings.Infrastructure.Services.ExcelTemplate;
public class ExcelTemplateService(ILogger<ExcelTemplateService> logger) : IExcelTemplateService
{
    public byte[] CreateImportTemplate()
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Import Template");

        var headers = new string[]
        {
        // Authority & Taxonomy
        "Authority Name", "Year", "Family Name", "Genus Name", "Species Type Name",
        "Species Type Description", "Scientific Name", "Name", "EU Name", "Full Name",
        "Turkish Name", "English Name", "Turkish Names Trakel", "Trakel", "Kocak Name",
        "Hesselbarth Name",

        // Location
        "Province Name", "Province Code", "Square Ref", "Square Latitude", "Square Longitude",
        "Latitude", "Longitude", "Decimal Degrees", "Degrees Minutes Seconds", "Decimal Minutes",
        "UTM Coordinates", "MGRS Coordinates", "Altitude 1", "Altitude 2", "UTM Reference",
        "Coordinate Precision Level",

        // Observer
        "Observer Name", "Observer Surname", "Observer Full Name",

        // Observation Details
        "Sex", "Observation Date", "Life Stage", "Number Seen", "Notes", "Source",
        "Location Info"
        };

        // Set headers and style
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cells[1, i + 1];
            cell.Value = headers[i];
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
            cell.Style.Font.Bold = true;
        }

        // Add validation and formatting
        for (int col = 1; col <= headers.Length; col++)
        {
            // Auto fit columns
            worksheet.Column(col).AutoFit();

            // Get Excel column letter
            string columnLetter = GetExcelColumnName(col);

            // Add validation rules for specific columns
            switch (headers[col - 1])
            {
                case "Sex":
                    var sexValidation = worksheet.DataValidations.AddListValidation($"{columnLetter}2:{columnLetter}1000");
                    foreach (var value in new[] { "Male", "Female", "Unknown" })
                    {
                        sexValidation.Formula.Values.Add(value);
                    }
                    break;

                case "Life Stage":
                    var stageValidation = worksheet.DataValidations.AddListValidation($"{columnLetter}2:{columnLetter}1000");
                    foreach (var value in new[] { "Egg", "Larva", "Pupa", "Adult" })
                    {
                        stageValidation.Formula.Values.Add(value);
                    }
                    break;

                case "Coordinate Precision Level":
                    var precisionValidation = worksheet.DataValidations.AddListValidation($"{columnLetter}2:{columnLetter}1000");
                    foreach (var value in new[] { "High", "Medium", "Low" })
                    {
                        precisionValidation.Formula.Values.Add(value);
                    }
                    break;
            }

            // Date format for Observation Date
            if (headers[col - 1] == "Observation Date")
            {
                worksheet.Column(col).Style.Numberformat.Format = "yyyy-mm-dd";
            }

            // Number format for coordinates
            if (headers[col - 1].Contains("Latitude") || headers[col - 1].Contains("Longitude"))
            {
                worksheet.Column(col).Style.Numberformat.Format = "0.000000";
            }
        }

        // Freeze the header row
        worksheet.View.FreezePanes(2, 1);

        // Add instructions sheet
        var instructionsSheet = package.Workbook.Worksheets.Add("Instructions");
        instructionsSheet.Cells["A1"].Value = "Import Instructions";
        instructionsSheet.Cells["A1"].Style.Font.Size = 14;
        instructionsSheet.Cells["A1"].Style.Font.Bold = true;

        var instructions = new[]
        {
        "1. Do not modify or delete the header row",
        "2. All dates should be in YYYY-MM-DD format",
        "3. Coordinates should use decimal points, not commas",
        "4. Required fields:",
        "   - Scientific Name",
        "   - Observation Date",
        "   - Location (Latitude/Longitude or Square Ref)",
        "5. For Sex, use: Male, Female, or Unknown",
        "6. For Life Stage, use: Egg, Larva, Pupa, or Adult",
        "7. Numbers should not contain any special characters"
    };

        for (int i = 0; i < instructions.Length; i++)
        {
            instructionsSheet.Cells[i + 3, 1].Value = instructions[i];
        }

        instructionsSheet.Column(1).AutoFit();

        logger.LogInformation("Excel template created successfully");

        return package.GetAsByteArray();
    }
    public byte[] CreateImportTemplateWithMockData()
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Import Template");

        var headers = new string[]
        {
        // Authority & Taxonomy
        "Authority Name", "Year", "Family Name", "Genus Name", "Species Type Name",
        "Species Type Description", "Scientific Name", "Name", "EU Name", "Full Name",
        "Turkish Name", "English Name", "Turkish Names Trakel", "Trakel", "Kocak Name",
        "Hesselbarth Name",

        // Location
        "Province Name", "Province Code", "Square Ref", "Square Latitude", "Square Longitude",
        "Latitude", "Longitude", "Decimal Degrees", "Degrees Minutes Seconds", "Decimal Minutes",
        "UTM Coordinates", "MGRS Coordinates", "Altitude 1", "Altitude 2", "UTM Reference",
        "Coordinate Precision Level",

        // Observer
        "Observer Name", "Observer Surname", "Observer Full Name",

        // Observation Details
        "Sex", "Observation Date", "Life Stage", "Number Seen", "Notes", "Source",
        "Location Info"
        };

        // Set headers and style
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cells[1, i + 1];
            cell.Value = headers[i];
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
            cell.Style.Font.Bold = true;
        }

        // Example rows
        var exampleData = new object[][]
        {
            new object[] {
                // Authority & Taxonomy
                "Linnaeus", 1758, "Pieridae", "Pieris", "Butterfly",
                "White butterfly species", "Pieris brassicae", "Large White", "Large White", "Pieris brassicae (Linnaeus, 1758)",
                "Lahana Kelebeği", "Large White", "Lahana Kelebeği", "Common species", "Alternative name",
                "Historical name",
                // Location
                "Ankara", 6, "B4", 39.5m, 32.5m,
                39.925533m, 32.866287m, "39.925533, 32.866287", "39°55'31.9\"N 32°51'58.6\"E", "39°55.533'N 32°51.977'E",
                "36S 539144 4419221", "36S YJ 39144 19221", 850m, 900m, "36S YJ",
                1,
                // Observer
                "John", "Doe", "John Doe",
                // Observation Details
                "Female", "2024-03-20", "Adult", 1, "Feeding on flowers", "Field observation", "Eastern slope"
            },
            new object[] {
                // Authority & Taxonomy
                "Fabricius", 1775, "Nymphalidae", "Vanessa", "Butterfly",
                "Painted lady butterfly", "Vanessa cardui", "Painted Lady", "Painted Lady", "Vanessa cardui (Fabricius, 1775)",
                "Diken Kelebeği", "Painted Lady", "Diken Kelebeği", "Migratory species", "Second name",
                "Old reference",
                // Location
                "İstanbul", 34, "A3", 41.0m, 29.0m,
                41.015137m, 28.979530m, "41.015137, 28.979530", "41°00'54.5\"N 28°58'46.3\"E", "41°0.908'N 28°57.772'E",
                "35T 683453 4543121", "35T CG 83453 43121", 100m, 150m, "35T CG",
                2,
                // Observer
                "Jane", "Smith", "Jane Smith",
                // Observation Details
                "Male", "2024-03-21", "Adult", 3, "Flying across garden", "Photo observation", "Garden area"
            },
            new object[] {
                // Authority & Taxonomy
                "Staudinger", 1871, "Lycaenidae", "Lycaena", "Butterfly",
                "Copper butterfly", "Lycaena phlaeas", "Small Copper", "Small Copper", "Lycaena phlaeas (Linnaeus, 1761)",
                "Benekli Bakır", "Small Copper", "Benekli Bakır", "Common copper", "Third name",
                "Traditional name",
                // Location
                "İzmir", 35, "B3", 38.4m, 27.1m,
                38.423734m, 27.142826m, "38.423734, 27.142826", "38°25'25.4\"N 27°08'34.2\"E", "38°25.423'N 27°08.571'E",
                "35S 500234 4254321", "35S BC 00234 54321", 20m, 50m, "35S BC",
                3,
                // Observer
                "Michael", "Brown", "Michael Brown",
                // Observation Details
                "Unknown", "2024-03-22", "Pupa", 2, "Found on plant stem", "Research study", "Coastal area"
            }
        };

        for (int row = 0; row < exampleData.Length; row++)
        {
            for (int col = 0; col < exampleData[row].Length; col++)
            {
                worksheet.Cells[row + 2, col + 1].Value = exampleData[row][col];
            }
        }

        // Add validation and formatting
        for (int col = 1; col <= headers.Length; col++)
        {
            // Auto fit columns
            worksheet.Column(col).AutoFit();

            // Get Excel column letter
            string columnLetter = GetExcelColumnName(col);

            // Add validation rules for specific columns
            switch (headers[col - 1])
            {
                case "Sex":
                    var sexValidation = worksheet.DataValidations.AddListValidation($"{columnLetter}5:{columnLetter}1000");
                    foreach (var value in new[] { "Male", "Female", "Unknown" })
                    {
                        sexValidation.Formula.Values.Add(value);
                    }
                    break;

                case "Life Stage":
                    var stageValidation = worksheet.DataValidations.AddListValidation($"{columnLetter}5:{columnLetter}1000");
                    foreach (var value in new[] { "Egg", "Larva", "Pupa", "Adult" })
                    {
                        stageValidation.Formula.Values.Add(value);
                    }
                    break;

                case "Coordinate Precision Level":
                    var precisionValidation = worksheet.DataValidations.AddListValidation($"{columnLetter}5:{columnLetter}1000");
                    foreach (var value in new[] { "High", "Medium", "Low" })
                    {
                        precisionValidation.Formula.Values.Add(value);
                    }
                    break;
            }

            // Date format for Observation Date
            if (headers[col - 1] == "Observation Date")
            {
                worksheet.Column(col).Style.Numberformat.Format = "yyyy-mm-dd";
            }

            // Number format for coordinates
            if (headers[col - 1].Contains("Latitude") || headers[col - 1].Contains("Longitude"))
            {
                worksheet.Column(col).Style.Numberformat.Format = "0.000000";
            }
        }

        // Freeze the header row
        worksheet.View.FreezePanes(2, 1);

        // Add instructions sheet
        var instructionsSheet = package.Workbook.Worksheets.Add("Instructions");
        instructionsSheet.Cells["A1"].Value = "Import Instructions";
        instructionsSheet.Cells["A1"].Style.Font.Size = 14;
        instructionsSheet.Cells["A1"].Style.Font.Bold = true;

        var instructions = new[]
        {
        "1. Do not modify or delete the header row",
        "2. All dates should be in YYYY-MM-DD format",
        "3. Coordinates should use decimal points, not commas",
        "4. Required fields:",
        "   - Scientific Name",
        "   - Observation Date",
        "   - Location (Latitude/Longitude or Square Ref)",
        "5. For Sex, use: Male, Female, or Unknown",
        "6. For Life Stage, use: Egg, Larva, Pupa, or Adult",
        "7. Numbers should not contain any special characters"
    };

        for (int i = 0; i < instructions.Length; i++)
        {
            instructionsSheet.Cells[i + 3, 1].Value = instructions[i];
        }

        instructionsSheet.Column(1).AutoFit();

        return package.GetAsByteArray();
    }
    private string GetExcelColumnName(int columnNumber)
    {
        string columnName = "";
        while (columnNumber > 0)
        {
            int modulo = (columnNumber - 1) % 26;
            columnName = Convert.ToChar('A' + modulo) + columnName;
            columnNumber = (columnNumber - modulo) / 26;
        }
        return columnName;
    }

}
