using BioWings.Infrastructure.Services.ExcelTemplate;
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
}
