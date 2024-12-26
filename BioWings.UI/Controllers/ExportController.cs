using BioWings.Application.Results;
using BioWings.UI.ViewModels.ExportViewModels;
using BioWings.UI.ViewModels.ObservationViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BioWings.UI.Controllers;
public class ExportController(IHttpClientFactory httpClientFactory, ILogger<ObservationController> logger) : Controller
{
    public async Task<IActionResult> Index()
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://localhost:7128/api/Observations/Count");
        int observationRecordCount = 0;
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<ObservationGetCountViewModel>>(content);
            if (apiResponse.IsSuccess && apiResponse.Data != null)
            {
                observationRecordCount=apiResponse.Data.Count;
            }
        }
        ViewBag.observationCount=observationRecordCount;

        return View();
    }
    public async Task<IActionResult> GetColumnNames()
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://localhost:7128/api/Exports/GetColumnNames");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var columns = JsonConvert.DeserializeObject<ApiResponse<List<ExportColumnViewModel>>>(content);
            logger.LogInformation($"Deserialized Columns: {JsonConvert.SerializeObject(columns)}");
            if (columns.IsSuccess && columns.Data != null)
            {
                var groupedColumns = columns.Data
                    .GroupBy(x => x.TableName)
                    .ToDictionary(g => g.Key, g => g.ToList());

                return Json(new ApiResponse<Dictionary<string, List<ExportColumnViewModel>>>
                {
                    IsSuccess = true,
                    Data = groupedColumns
                });
            }
        }

        return Json(new ApiResponse<Dictionary<string, List<ExportColumnViewModel>>>
        {
            IsSuccess = false,
            ErrorList = new List<string> { "An error occurred while fetching columns" }
        });
    }
    public async Task<IActionResult> ExportData([FromBody] ExportCreateViewModel exportCreateViewModel)
    {
        using (var httpClient = httpClientFactory.CreateClient())
        {
            // API'ye request gönder
            var response = await httpClient.PostAsJsonAsync("https://localhost:7128/api/Exports/ExportData", exportCreateViewModel);

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
