using BioWings.Application.DTOs.ExportDtos;
using BioWings.Domain.Entities;

namespace BioWings.Application.Services;
public interface IExcelExportService
{
    byte[] ExportToExcel(IEnumerable<Observation> observations, List<ExpertColumnInfo> columns);
}
