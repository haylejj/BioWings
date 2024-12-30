using BioWings.Application.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BioWings.WebAPI.Controllers;
[EnableCors("AllowMvcApp")]
public class ExcelTemplateController(IExcelTemplateService excelTemplateService) : BaseController
{
    [HttpGet("download")]
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
