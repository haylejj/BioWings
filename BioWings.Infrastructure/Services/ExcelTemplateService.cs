using BioWings.Application.Services;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace BioWings.Infrastructure.Services;
public class ExcelTemplateService(ILogger<ExcelTemplateService> logger) : IExcelTemplateService
{
    public byte[] CreateImportTemplate()
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Import Template");

        var headers = new string[]
        {
        // Authority & Taxonomy
        "Authority Name", "Year", "Family Name", "Genus Name","Subspecies Name",
        "Scientific Name", "Name", "EU Name", "Full Name",
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

        //    // Add instructions sheet
        //    var instructionsSheet = package.Workbook.Worksheets.Add("Instructions");
        //    instructionsSheet.Cells["A1"].Value = "Import Instructions";
        //    instructionsSheet.Cells["A1"].Style.Font.Size = 14;
        //    instructionsSheet.Cells["A1"].Style.Font.Bold = true;

        //    var instructions = new[]
        //    {
        //    "1. Do not modify or delete the header row",
        //    "2. All dates should be in YYYY-MM-DD format",
        //    "3. Coordinates should use decimal points, not commas",
        //    "4. Required fields:",
        //    "   - Scientific Name",
        //    "   - Observation Date",
        //    "   - Location (Latitude/Longitude or Square Ref)",
        //    "5. For Sex, use: Male, Female, or Unknown",
        //    "6. For Life Stage, use: Egg, Larva, Pupa, or Adult",
        //    "7. Numbers should not contain any special characters"
        //};

        //    for (int i = 0; i < instructions.Length; i++)
        //    {
        //        instructionsSheet.Cells[i + 3, 1].Value = instructions[i];
        //    }

        //    instructionsSheet.Column(1).AutoFit();

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
        "Authority Name", "Year", "Family Name", "Genus Name","Subspecies Name",
        "Scientific Name", "Name", "EU Name", "Full Name",
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
                "Linnaeus", 1758, "Pieridae", "Pieris","brassicae major",
                "Pieris brassicae", "Large White", "Large White", "Pieris brassicae (Linnaeus, 1758)",
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
                "Fabricius", 1775, "Nymphalidae", "Vanessa","cardui indica",
                "Vanessa cardui", "Painted Lady", "Painted Lady", "Vanessa cardui (Fabricius, 1775)",
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
                "Staudinger", 1871, "Lycaenidae", "Lycaena","phlaeas eleus",
                "Lycaena phlaeas", "Small Copper", "Small Copper", "Lycaena phlaeas (Linnaeus, 1761)",
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
            },
            new object[] {
                // Authority & Taxonomy
                "Ochsenheimer", 1808, "Papilionidae", "Parnassius","apollo melliculus",
                "Parnassius apollo", "Apollo", "Apollo", "Parnassius apollo (Linnaeus, 1758)",
                "Apollon Kelebeği", "Apollo", "Apollon", "Mountain species", "Fourth name",
                "Mountain apollo",
                // Location
                "Bolu", 14, "A4", 40.5m, 31.5m,
                40.739589m, 31.611561m, "40.739589, 31.611561", "40°44'22.5\"N 31°36'41.6\"E", "40°44.376'N 31°36.693'E",
                "36T 392844 4511221", "36T XG 92844 11221", 2300m, 2400m, "36T XG",
                0, // Exact coordinate
                // Observer
                "Sarah", "Wilson", "Sarah Wilson",
                // Observation Details
                "Female", "2024-06-15", "Adult", 2, "Nectaring on mountain flowers", "Research observation", "Alpine meadow"
            },
            new object[] {
                // Authority & Taxonomy
                "Freyer", 1831, "Sphingidae", "Acherontia","atropos syriaca",
                "Acherontia atropos", "Death's-head Hawkmoth", "Death's-head Hawkmoth", "Acherontia atropos (Linnaeus, 1758)",
                "Kuru Kafa Güvesi", "Death's-head Hawkmoth", "Kuru Kafa", "Migrant moth", "Alternative name",
                "Historical reference",
                // Location
                "Antalya", 7, "C3", 36.9m, 30.7m,
                36.912812m, 30.688770m, "36.912812, 30.688770", "36°54'46.1\"N 30°41'19.6\"E", "36°54.768'N 30°41.326'E",
                "36S 249144 4089221", "36S WF 49144 89221", 50m, 100m, "36S WF",
                1,
                // Observer
                "David", "Miller", "David Miller",
                // Observation Details
                "Unknown", "2024-07-20", "Adult", 1, "Attracted to light", "Light trap", "Urban garden"
            },
            new object[] {
                // Authority & Taxonomy
                "Esper", 1777, "Saturniidae", "Saturnia","pavonia meridionalis",
                "Saturnia pavonia", "Emperor Moth", "Emperor Moth", "Saturnia pavonia (Linnaeus, 1758)",
                "Tavus Güvesi", "Small Emperor Moth", "Tavus Güvesi", "Common moth", "Local name",
                "Traditional reference",
                // Location
                "Trabzon", 61, "A7", 41.0m, 39.7m,
                41.005270m, 39.724791m, "41.005270, 39.724791", "41°00'18.9\"N 39°43'29.2\"E", "41°00.315'N 39°43.487'E",
                "37T 539144 4539221", "37T GH 39144 39221", 1200m, 1300m, "37T GH",
                2,
                // Observer
                "Emma", "Clark", "Emma Clark",
                // Observation Details
                "Male", "2024-04-10", "Adult", 3, "Males flying in afternoon", "Visual observation", "Forest edge"
            },
            new object[] {
                // Authority & Taxonomy
                "Berge", 1842, "Nymphalidae", "Argynnis","paphia dives",
                "Argynnis paphia", "Silver-washed Fritillary", "Silver-washed Fritillary", "Argynnis paphia (Linnaeus, 1758)",
                "Büyük İnci", "Silver-washed Fritillary", "Büyük İnci", "Forest species", "Local variant",
                "Classical name",
                // Location
                "Artvin", 8, "A9", 41.2m, 41.8m,
                41.182474m, 41.818707m, "41.182474, 41.818707", "41°10'56.9\"N 41°49'07.3\"E", "41°10.949'N 41°49.122'E",
                "37T 739144 4559221", "37T TF 39144 59221", 800m, 900m, "37T TF",
                1,
                // Observer
                "James", "Taylor", "James Taylor",
                // Observation Details
                "Female", "2024-07-05", "Adult", 4, "Ovipositing on violet leaves", "Behavioral study", "Mixed woodland"
            },
            new object[] {
                // Authority & Taxonomy
                "Schiffermüller", 1775, "Lycaenidae", "Polyommatus","icarus zelleri",
                "Polyommatus icarus", "Common Blue", "Common Blue", "Polyommatus icarus (Rottemburg, 1775)",
                "Çokgözlü Mavi", "Common Blue", "Çokgözlü Mavi", "Widespread species", "Regional name",
                "Standard reference",
                // Location
                "Muğla", 48, "B2", 37.2m, 28.3m,
                37.215726m, 28.363352m, "37.215726, 28.363352", "37°12'56.6\"N 28°21'48.1\"E", "37°12.944'N 28°21.801'E",
                "35S 539144 4119221", "35S NF 39144 19221", 150m, 200m, "35S NF",
                2,
                // Observer
                "Lisa", "Anderson", "Lisa Anderson",
                // Observation Details
                "Male", "2024-05-25", "Adult", 6, "Courtship behavior observed", "Field study", "Mediterranean scrub"
            },
            new object[] {
                // Authority & Taxonomy
                "Poda", 1761, "Pieridae", "Colias","crocea helice",
                 "Colias crocea", "Clouded Yellow", "Clouded Yellow", "Colias crocea (Geoffroy, 1785)",
                "Sarı Azamet", "Clouded Yellow", "Sarı Azamet", "Migrant species", "Southern form",
                "Migration reference",
                // Location
                "Mersin", 33, "C4", 36.8m, 34.6m,
                36.812100m, 34.641479m, "36.812100, 34.641479", "36°48'43.6\"N 34°38'29.3\"E", "36°48.726'N 34°38.489'E",
                "36S 639144 4079221", "36S VF 39144 79221", 5m, 10m, "36S VF",
                0,
                // Observer
                "Robert", "Johnson", "Robert Johnson",
                // Observation Details
                "Female", "2024-09-15", "Adult", 8, "Migration movement southward", "Transect count", "Coastal dunes"
            },
            new object[] {
                // Authority & Taxonomy
                "Verity", 1905, "Hesperiidae", "Thymelicus","lineola centralasiae",
                "Thymelicus lineola", "Essex Skipper", "Essex Skipper", "Thymelicus lineola (Ochsenheimer, 1808)",
                "Çizgili Zıpzıp", "Essex Skipper", "Çizgili Zıpzıp", "Grassland species", "Eastern form",
                "Modern reference",
                // Location
                "Samsun", 55, "A6", 41.3m, 36.3m,
                41.292782m, 36.331039m, "41.292782, 36.331039", "41°17'34.0\"N 36°19'51.7\"E", "41°17.567'N 36°19.862'E",
                "37T 339144 4579221", "37T PF 39144 79221", 300m, 350m, "37T PF",
                3,
                // Observer
                "Maria", "Garcia", "Maria Garcia",
                // Observation Details
                "Unknown", "2024-06-30", "Larva", 5, "Feeding on grass stems", "Ecological study", "Meadow habitat"
            },
            new object[] {
                // Authority & Taxonomy
                "Hübner", 1819, "Nymphalidae", "Nymphalis", "polychloros fervida",
                "Nymphalis polychloros", "Large Tortoiseshell", "Large Tortoiseshell", "Nymphalis polychloros (Linnaeus, 1758)",
                "Büyük Sırçalı", "Large Tortoiseshell", "Büyük Sırçalı", "Forest species", "Regional variant",
                "Classical form",
                // Location
                "Kayseri", 38, "B5", 38.7m, 35.5m,
                38.722909m, 35.487267m, "38.722909, 35.487267", "38°43'22.5\"N 35°29'14.2\"E", "38°43.375'N 35°29.236'E",
                "36S 739144 4289221", "36S KF 39144 89221", 1500m, 1600m, "36S KF",
                2,
                // Observer
                "Lucas", "Wright", "Lucas Wright",
                // Observation Details
                "Female", "2024-08-10", "Adult", 3, "Territorial behavior", "Field research", "Mountain forest"
            },
            new object[] {
                // Authority & Taxonomy
                "Latreille", 1809, "Sphingidae", "Hyles", "euphorbiae tithymali",
                 "Hyles euphorbiae", "Spurge Hawk-moth", "Spurge Hawk-moth", "Hyles euphorbiae (Linnaeus, 1758)",
                "Sütleğen Kartalkuyruğu", "Spurge Hawk-moth", "Sütleğen Kartalkuyruğu", "Night flyer", "Southern type",
                "Mediterranean form",
                // Location
                "İzmir", 35, "B2", 38.4m, 27.1m,
                38.418891m, 27.128889m, "38.418891, 27.128889", "38°25'08.0\"N 27°07'44.0\"E", "38°25.133'N 27°07.733'E",
                "35S 449144 4259221", "35S MF 49144 59221", 100m, 150m, "35S MF",
                1,
                // Observer
                "Sophie", "Martinez", "Sophie Martinez",
                // Observation Details
                "Male", "2024-06-20", "Adult", 2, "On Euphorbia plants", "Night survey", "Coastal scrub"
            },
            new object[] {
                // Authority & Taxonomy
                "Bergsträsser", 1779, "Lycaenidae", "Lysandra", "bellargus coridon",
                "Lysandra bellargus", "Adonis Blue", "Adonis Blue", "Lysandra bellargus (Rottemburg, 1775)",
                "Gökyakut Mavisi", "Adonis Blue", "Gökyakut Mavisi", "Chalk grassland species", "Local race",
                "Western form",
                // Location
                "Eskişehir", 26, "B4", 39.7m, 30.5m,
                39.776665m, 30.520555m, "39.776665, 30.520555", "39°46'36.0\"N 30°31'14.0\"E", "39°46.600'N 30°31.233'E",
                "36S 279144 4399221", "36S HF 79144 99221", 800m, 850m, "36S HF",
                0,
                // Observer
                "Oliver", "Chen", "Oliver Chen",
                // Observation Details
                "Female", "2024-05-15", "Adult", 5, "Nectaring behavior", "Population study", "Steppe grassland"
            },
            new object[] {
                // Authority & Taxonomy
                "Denis", 1775, "Papilionidae", "Zerynthia", "polyxena cassandra",
                "Zerynthia polyxena", "Southern Festoon", "Southern Festoon", "Zerynthia polyxena (Denis & Schiffermüller, 1775)",
                "Güzel Kurt", "Southern Festoon", "Güzel Kurt", "Spring species", "Eastern race",
                "Spring form",
                // Location
                "Bursa", 16, "B2", 40.1m, 29.0m,
                40.182863m, 29.066857m, "40.182863, 29.066857", "40°10'58.3\"N 29°04'00.7\"E", "40°10.972'N 29°04.011'E",
                "35T 679144 4449221", "35T PF 79144 49221", 700m, 750m, "35T PF",
                1,
                // Observer
                "Isabella", "Kumar", "Isabella Kumar",
                // Observation Details
                "Male", "2024-04-05", "Adult", 4, "Early spring emergence", "Phenology study", "Mixed woodland"
            },
            new object[] {
                // Authority & Taxonomy
                "Butler", 1869, "Geometridae", "Archiearis", "parthenias lucida",
                 "Archiearis parthenias", "Orange Underwing", "Orange Underwing", "Archiearis parthenias (Linnaeus, 1761)",
                "Turuncu Altkanat", "Orange Underwing", "Turuncu Altkanat", "Early spring moth", "Northern variant",
                "Spring variant",
                // Location
                "Rize", 53, "A8", 41.0m, 40.5m,
                41.025555m, 40.517777m, "41.025555, 40.517777", "41°01'32.0\"N 40°31'04.0\"E", "41°01.533'N 40°31.067'E",
                "37T 589144 4549221", "37T RF 89144 49221", 1800m, 1900m, "37T RF",
                2,
                // Observer
                "Nathan", "Price", "Nathan Price",
                // Observation Details
                "Unknown", "2024-03-25", "Adult", 6, "Flying around birch trees", "Early spring monitoring", "Subalpine forest"
            },
            new object[] {
                // Authority & Taxonomy
                "Pallas", 1771, "Papilionidae", "Iphiclides", "podalirius feisthamelii",
                 "Iphiclides podalirius", "Scarce Swallowtail", "Scarce Swallowtail", "Iphiclides podalirius (Linnaeus, 1758)",
                "Erik Kırlangıçkuyruğu", "Scarce Swallowtail", "Erik Kırlangıçkuyruğu", "Fruit tree associate", "Southern population",
                "Mediterranean type",
                // Location
                "Aydın", 9, "B3", 37.8m, 27.8m,
                37.845555m, 27.845555m, "37.845555, 27.845555", "37°50'44.0\"N 27°50'44.0\"E", "37°50.733'N 27°50.733'E",
                "35S 559144 4189221", "35S LF 59144 89221", 250m, 300m, "35S LF",
                1,
                // Observer
                "Victoria", "Lee", "Victoria Lee",
                // Observation Details
                "Female", "2024-07-12", "Adult", 2, "Oviposition on plum tree", "Breeding study", "Orchard habitat"
            },
            new object[] {
                // Authority & Taxonomy
                "Stoll", 1782, "Lasiocampidae", "Eriogaster", "lanestris arbusculae",
                "Eriogaster lanestris", "Small Eggar", "Small Eggar", "Eriogaster lanestris (Linnaeus, 1758)",
                "Küçük İplikçi", "Small Eggar", "Küçük İplikçi", "Early spring species", "Mountain form",
                "Alpine variant",
                // Location
                "Erzurum", 25, "B6", 39.9m, 41.2m,
                39.908889m, 41.276667m, "39.908889, 41.276667", "39°54'32.0\"N 41°16'36.0\"E", "39°54.533'N 41°16.600'E",
                "37T 689144 4419221", "37T TF 89144 19221", 1900m, 2000m, "37T TF",
                3,
                // Observer
                "Benjamin", "Scott", "Benjamin Scott",
                // Observation Details
                "Male", "2024-04-18", "Adult", 3, "Larval web found", "Life cycle study", "Mountain birch forest"
            },
            new object[] {
                // Authority & Taxonomy
                "Rambur", 1839, "Sphingidae", "Proserpinus", "proserpina schmidti",
                "Proserpinus proserpina", "Willowherb Hawkmoth", "Willowherb Hawkmoth", "Proserpinus proserpina (Pallas, 1772)",
                "Yakıotu Kartalkuyruğu", "Willowherb Hawkmoth", "Yakıotu Kartalkuyruğu", "Protected species", "Eastern population",
                "Anatolian form",
                // Location
                "Konya", 42, "B4", 37.8m, 32.4m,
                37.871666m, 32.484722m, "37.871666, 32.484722", "37°52'18.0\"N 32°29'05.0\"E", "37°52.300'N 32°29.083'E",
                "36S 559144 4189221", "36S MF 59144 89221", 1000m, 1100m, "36S MF",
                0,
                // Observer
                "Emily", "Rodriguez", "Emily Rodriguez",
                // Observation Details
                "Female", "2024-06-25", "Adult", 1, "At light trap", "Conservation monitoring", "Steppe wetland"
            },
            new object[] {
                // Authority & Taxonomy
                "Zeller", 1847, "Zygaenidae", "Zygaena", "filipendulae anatolica",
                "Zygaena filipendulae", "Six-spot Burnet", "Six-spot Burnet", "Zygaena filipendulae (Linnaeus, 1758)",
                "Altı Benekli Kızılkanat", "Six-spot Burnet", "Altı Benekli Kızılkanat", "Meadow species", "Turkish race",
                "Local subspecies",
                // Location
                "Denizli", 20, "B2", 37.7m, 29.0m,
                37.773889m, 29.086944m, "37.773889, 29.086944", "37°46'26.0\"N 29°05'13.0\"E", "37°46.433'N 29°05.217'E",
                "35S 679144 4179221", "35S NF 79144 79221", 850m, 900m, "35S NF",
                1,
                // Observer
                "William", "Zhang", "William Zhang",
                // Observation Details
                "Male", "2024-07-30", "Adult", 7, "Mating pair observed", "Behavior study", "Flower-rich meadow"
            },
            new object[] {
                // Authority & Taxonomy
                "Borkhausen", 1788, "Noctuidae", "Catocala", "nupta orientalis",
                "Catocala nupta", "Red Underwing", "Red Underwing", "Catocala nupta (Linnaeus, 1767)",
                "Kırmızı Altkanat", "Red Underwing", "Kırmızı Altkanat", "Night-flying moth", "Eastern subspecies",
                "River valley form",
                // Location
                "Edirne", 22, "A2", 41.6m, 26.5m,
                41.677222m, 26.555833m, "41.677222, 26.555833", "41°40'38.0\"N 26°33'21.0\"E", "41°40.633'N 26°33.350'E",
                "35T 459144 4609221", "35T PE 59144 09221", 50m, 100m, "35T PE",
                2,
                // Observer
                "Alexandra", "Brown", "Alexandra Brown",
                // Observation Details
                "Female", "2024-08-20", "Adult", 4, "Resting on willow trunk", "Nocturnal survey", "Riparian woodland"
            },
            new object[] {
               // Authority & Taxonomy
               "Geyer", 1832, "Nymphalidae", "Melitaea", "phoebe telona",
               "Melitaea phoebe", "Knapweed Fritillary", "Knapweed Fritillary", "Melitaea phoebe (Denis & Schiffermüller, 1775)",
               "Büyük İparhan", "Knapweed Fritillary", "Büyük İparhan", "Mediterranean species", "Southern population",
               "Historical variant",
               // Location
               "Adana", 1, "C5", 37.0m, 35.3m,
               37.041667m, 35.358889m, "37.041667, 35.358889", "37°02'30.0\"N 35°21'32.0\"E", "37°02.500'N 35°21.533'E",
               "36S 739144 4099221", "36S VF 39144 99221", 150m, 200m, "36S VF",
               1,
               // Observer
               "Marcus", "Thompson", "Marcus Thompson",
               // Observation Details
               "Female", "2024-05-08", "Adult", 4, "Nectaring on thistles", "Habitat survey", "Mediterranean scrub"
            },
            new object[] {
               // Authority & Taxonomy
               "Duponchel", 1835, "Saturniidae", "Actias", "selene artemis",
               "Actias selene", "Moon Moth", "Moon Moth", "Actias selene (Hübner, 1807)",
               "Ay Güvesi", "Moon Moth", "Ay Güvesi", "Asian species", "Western population",
               "Anatolian race",
               // Location
               "Hatay", 31, "C6", 36.2m, 36.1m,
               36.252778m, 36.158333m, "36.252778, 36.158333", "36°15'10.0\"N 36°09'30.0\"E", "36°15.167'N 36°09.500'E",
               "37S 259144 4009221", "37S BF 59144 09221", 400m, 450m, "37S BF",
               0,
               // Observer
               "Rachel", "Kim", "Rachel Kim",
               // Observation Details
               "Male", "2024-06-12", "Adult", 2, "Light trap capture", "Biodiversity study", "Mixed forest"
            },
            new object[] {
               // Authority & Taxonomy
               "Kollar", 1848, "Pieridae", "Anthocharis", "cardamines phoenissa",
               "Anthocharis cardamines", "Orange-tip", "Orange-tip", "Anthocharis cardamines (Linnaeus, 1758)",
               "Turuncu Süslü", "Orange-tip", "Turuncu Süslü", "Spring butterfly", "Eastern form",
               "Spring emergence",
               // Location
               "Balıkesir", 10, "B1", 39.6m, 27.9m,
               39.648611m, 27.886389m, "39.648611, 27.886389", "39°38'55.0\"N 27°53'11.0\"E", "39°38.917'N 27°53.183'E",
               "35T 559144 4389221", "35T NF 59144 89221", 300m, 350m, "35T NF",
               2,
               // Observer
               "Daniel", "Morgan", "Daniel Morgan",
               // Observation Details
               "Female", "2024-04-15", "Adult", 3, "Egg-laying observed", "Life cycle study", "Woodland edge"
            },
            new object[] {
               // Authority & Taxonomy
               "Ménétriés", 1832, "Lycaenidae", "Tomares", "callimachus hafiz",
              "Tomares callimachus", "Callimachus", "Callimachus", "Tomares callimachus (Eversmann, 1848)",
               "Küçük Sevbeni", "Callimachus", "Küçük Sevbeni", "Early spring species", "Turkish endemic",
               "Local variant",
               // Location
               "Nevşehir", 50, "B5", 38.6m, 34.7m,
               38.624167m, 34.714444m, "38.624167, 34.714444", "38°37'27.0\"N 34°42'52.0\"E", "38°37.450'N 34°42.867'E",
               "36S 639144 4279221", "36S KF 39144 79221", 1100m, 1200m, "36S KF",
               1,
               // Observer
               "Hannah", "Wilson", "Hannah Wilson",
               // Observation Details
               "Male", "2024-03-28", "Adult", 5, "Territory defense", "Behavioral research", "Rocky steppe"
            },
            new object[] {
               // Authority & Taxonomy
               "Staudinger", 1892, "Noctuidae", "Cucullia", "calendulae orientalis",
               "Cucullia calendulae", "Star-wort", "Star-wort", "Cucullia calendulae (Treitschke, 1835)",
               "Doğu Kapüşonlusu", "Star-wort", "Doğu Kapüşonlusu", "Night-flying moth", "Eastern subspecies",
               "Anatolian type",
               // Location
               "Van", 65, "B9", 38.5m, 43.4m,
               38.508333m, 43.374167m, "38.508333, 43.374167", "38°30'30.0\"N 43°22'27.0\"E", "38°30.500'N 43°22.450'E",
               "38S 339144 4259221", "38S EF 39144 59221", 1700m, 1800m, "38S EF",
               3,
               // Observer
               "Thomas", "Baker", "Thomas Baker",
               // Observation Details
               "Unknown", "2024-07-18", "Adult", 1, "Night observation", "Moth survey", "Mountain steppe"
            },
            new object[] {
               // Authority & Taxonomy
               "Herrich-Schäffer", 1851, "Hesperiidae", "Carcharodus", "alceae swinhoei",
               "Carcharodus alceae", "Mallow Skipper", "Mallow Skipper", "Carcharodus alceae (Esper, 1780)",
               "Hatmi Zıpzıpı", "Mallow Skipper", "Hatmi Zıpzıpı", "Garden species", "Asian form",
               "Garden variant",
               // Location
               "Manisa", 45, "B2", 38.6m, 27.4m,
               38.619444m, 27.428889m, "38.619444, 27.428889", "38°37'10.0\"N 27°25'44.0\"E", "38°37.167'N 27°25.733'E",
               "35S 459144 4279221", "35S MF 59144 79221", 250m, 300m, "35S MF",
               0,
               // Observer
               "Laura", "Collins", "Laura Collins",
               // Observation Details
               "Female", "2024-05-30", "Adult", 6, "Feeding on Malva", "Urban ecology", "Garden habitat"
            },
            new object[] {
               // Authority & Taxonomy
               "Moore", 1865, "Sphingidae", "Deilephila", "elpenor szechuana",
               "Deilephila elpenor", "Elephant Hawk-moth", "Elephant Hawk-moth", "Deilephila elpenor (Linnaeus, 1758)",
               "Büyük Fildişi", "Elephant Hawk-moth", "Büyük Fildişi", "Night-flying moth", "Eastern race",
               "Asian subspecies",
               // Location
               "Gaziantep", 27, "C5", 37.0m, 37.3m,
               37.066667m, 37.383333m, "37.066667, 37.383333", "37°04'00.0\"N 37°23'00.0\"E", "37°04.000'N 37°23.000'E",
               "37S 339144 4109221", "37S CF 39144 09221", 850m, 900m, "37S CF",
               2,
               // Observer
               "Christopher", "Park", "Christopher Park",
               // Observation Details
               "Male", "2024-06-28", "Adult", 3, "Attracted to light", "Urban moth survey", "Suburban garden"
            },
            new object[] {
               // Authority & Taxonomy
               "Ochsenheimer", 1816, "Nymphalidae", "Limenitis", "reducta schiffermuelleri",
               "Limenitis reducta", "Southern White Admiral", "Southern White Admiral", "Limenitis reducta (Staudinger, 1901)",
               "Küçük İparhan", "Southern White Admiral", "Küçük İparhan", "Woodland species", "Southern form",
               "Mediterranean type",
               // Location
               "Isparta", 32, "C3", 37.7m, 30.5m,
               37.766667m, 30.556944m, "37.766667, 30.556944", "37°46'00.0\"N 30°33'25.0\"E", "37°46.000'N 30°33.417'E",
               "36S 539144 4179221", "36S WF 39144 79221", 1000m, 1100m, "36S WF",
               1,
               // Observer
               "Sophia", "Adams", "Sophia Adams",
               // Observation Details
               "Female", "2024-07-08", "Adult", 2, "Territorial behavior", "Population study", "Mixed woodland"
            },
            new object[] {
               // Authority & Taxonomy
               "Staudinger", 1878, "Zygaenidae", "Jordanita", "graeca anatolica",
               "Jordanita graeca", "Forester Moth", "Forester Moth", "Jordanita graeca (Jordan, 1907)",
               "Yeşil Metal", "Forester Moth", "Yeşil Metal", "Day-flying moth", "Turkish endemic",
               "Endemic form",
               // Location
               "Tokat", 60, "B6", 40.3m, 36.5m,
               40.316667m, 36.554167m, "40.316667, 36.554167", "40°19'00.0\"N 36°33'15.0\"E", "40°19.000'N 36°33.250'E",
               "37T 439144 4459221", "37T PF 39144 59221", 600m, 650m, "37T PF",
               2,
               // Observer
               "Andrew", "Fisher", "Andrew Fisher",
               // Observation Details
               "Unknown", "2024-05-22", "Adult", 4, "Hilltopping behavior", "Species distribution", "Flowery meadow"
            },
            new object[] {
               // Authority & Taxonomy
               "Draudt", 1931, "Geometridae", "Scopula", "ornata subornata",
               "Scopula ornata", "Lace Border", "Lace Border", "Scopula ornata (Scopoli, 1763)",
               "Süslü Düzkanat", "Lace Border", "Süslü Düzkanat", "Chalk grassland species", "Eastern variant",
               "Grassland form",
               // Location
               "Çorum", 19, "B5", 40.5m, 34.9m,
               40.549722m, 34.953611m, "40.549722, 34.953611", "40°32'59.0\"N 34°57'13.0\"E", "40°32.983'N 34°57.217'E",
               "36T 639144 4489221", "36T LF 39144 89221", 800m, 850m, "36T LF",
               1,
               // Observer
               "Julia", "Cooper", "Julia Cooper",
               // Observation Details
               "Female", "2024-06-05", "Adult", 5, "Day-flying activity", "Grassland monitoring", "Calcareous grassland"
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

        //// Add instructions sheet
        //var instructionsSheet = package.Workbook.Worksheets.Add("Instructions");
        //instructionsSheet.Cells["A1"].Value = "Import Instructions";
        //instructionsSheet.Cells["A1"].Style.Font.Size = 14;
        //instructionsSheet.Cells["A1"].Style.Font.Bold = true;

        //var instructions = new[]
        //{
        //"1. Do not modify or delete the header row",
        //"2. All dates should be in YYYY-MM-DD format",
        //"3. Coordinates should use decimal points, not commas",
        //"4. Required fields:",
        //"   - Scientific Name",
        //"   - Observation Date",
        //"   - Location (Latitude/Longitude or Square Ref)",
        //"5. For Sex, use: Male, Female, or Unknown",
        //"6. For Life Stage, use: Egg, Larva, Pupa, or Adult",
        //"7. Numbers should not contain any special characters"
        //};

        //for (int i = 0; i < instructions.Length; i++)
        //{
        //    instructionsSheet.Cells[i + 3, 1].Value = instructions[i];
        //}

        //instructionsSheet.Column(1).AutoFit();

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
