using BioWings.Application.DTOs.ImportDtos;
using BioWings.Application.DTOs.ObservationDtos;
using Microsoft.AspNetCore.Http;

namespace BioWings.Application.Services;
public interface IExcelImportService
{
    List<ImportCreateDto> ImportFromExcel(IFormFile file);
}
