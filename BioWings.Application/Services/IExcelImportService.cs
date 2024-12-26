using BioWings.Application.DTOs.ImportDtos;
using Microsoft.AspNetCore.Http;

namespace BioWings.Application.Services;
public interface IExcelImportService
{
    Task<List<ImportCreateDto>> ImportFromExcelAsync(IFormFile file);
}
