using BioWings.Application.DTOs.ExportDtos;

namespace BioWings.UI.ViewModels.ExportViewModels;

public class ExportCreateViewModel
{
    public List<ExpertColumnInfo> Columns { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? RecordLimit { get; set; }
    public bool ExportAllDates { get; set; }
    public bool ExportAllRecords { get; set; }
}
