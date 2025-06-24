using BioWings.Application.Results;
using BioWings.Domain.Configuration;
using BioWings.UI.Areas.Admin.Models.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace BioWings.UI.Areas.Admin.Controllers;
[Authorize]
[Area("Admin")]
public class RoleController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options) : Controller
{
    private readonly string _baseUrl = options.Value.BaseUrl;
    public async Task<IActionResult> Index()
    {
        var client = httpClientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync($"{_baseUrl}/Roles");
        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"]="An error occurred while fetching the roles";
            return View();
        }
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ApiResponse<List<RoleGetViewModel>>>(content);
        if (data == null || data.Data == null || !data.Data.Any())
        {
            return View(new RoleViewModel { GetViewModels = new List<RoleGetViewModel>() });
        }
        var model = new RoleViewModel
        {
            GetViewModels = data.Data
        };
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Update(RoleViewModel model)
    {
        var client = httpClientFactory.CreateClient("ApiClient");
        var content = new StringContent(JsonConvert.SerializeObject(model.UpdateViewModel), Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"{_baseUrl}/Roles", content);
        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessageForUpdate"]="An error occurred while updating the role";
            return RedirectToAction("Index");
        }
        TempData["SuccessMessageForUpdate"] = "Rol başarıyla güncellendi.";
        return RedirectToAction("Index");
    }
}
