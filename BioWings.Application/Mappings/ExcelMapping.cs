namespace BioWings.Application.Mappings;
public class ExcelMapping
{
    public string SheetName { get; set; }
    public Dictionary<string, string[]> ColumnMappings { get; set; }
    public int HeaderRow { get; set; } = 1;
    public int DataStartRow { get; set; } = 2;
}
