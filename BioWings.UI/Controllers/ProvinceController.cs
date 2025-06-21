using BioWings.Application.Results;
using BioWings.Domain.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BioWings.UI.Controllers;
[Authorize]
public class ProvinceController(IHttpClientFactory httpClientFactory, ILogger<ProvinceController> logger, IOptions<ApiSettings> options) : Controller
{
    private readonly string _baseUrl = options.Value.BaseUrl;
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet("ExportData/{provinceId}")]
    public async Task<IActionResult> ExportData([FromRoute] int provinceId)
    {
        using (var httpClient = httpClientFactory.CreateClient("ApiClient"))
        {
            // API'ye request gönder
            var response = await httpClient.GetAsync($"{_baseUrl}/Exports/ExportObservationsByProvince/{provinceId}");

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"API error: {response.StatusCode}");
                return StatusCode((int)response.StatusCode, "Export failed");
            }

            var result = await response.Content.ReadFromJsonAsync<ServiceResult<byte[]>>();
            logger.LogInformation("Exported data successfully");
            return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"exportObservations_{DateTime.Now:yyyy-MM-dd_HH-mm}.xlsx");
        }

    }
}
