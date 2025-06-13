using BioWings.Application.Services;
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
[EnableCors("AllowMvcApp")]
public class ExcelTemplateController(IExcelTemplateService excelTemplateService) : BaseController
{
    [HttpGet("download")]
    [AuthorizeDefinition("Şablon İndirme", ActionType.Read, "Gözlem import şablonu indirme", AreaNames.Public)]
    public IActionResult DownloadTemplate()
    {
        var fileContents = excelTemplateService.CreateImportTemplateWithMockData();
        return File(
            fileContents,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "observation_import_template.xlsx"
        );
    }
    [HttpGet("download/species")]
    [AuthorizeDefinition("Şablon İndirme", ActionType.Read, "Tür import şablonu indirme", AreaNames.Public)]
    public IActionResult DownloadSpeciesTemplate()
    {
        var fileContents = excelTemplateService.CreateSpeciesTemplateWithMockData();
        return File(
            fileContents,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "species_import_template.xlsx"
        );
    }
}
