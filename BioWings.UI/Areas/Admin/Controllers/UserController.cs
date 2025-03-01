using BioWings.Application.Results;
using BioWings.UI.Areas.Admin.Models.User;
using BioWings.UI.ViewModels.CountryViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace BioWings.UI.Areas.Admin.Controllers;
[Area("Admin")]
public class UserController(IHttpClientFactory httpClientFactory) : Controller
{
    public async Task<IActionResult> Index()
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://localhost:7128/api/Users");
        if (!response.IsSuccessStatusCode)
        {
            ViewData["ErrorMessage"]="An error occurred while fetching the data";
            return View();
        }
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ApiResponse<List<UserGetViewModel>>>(content);
        //dropdown list for countries
        var countryClient = httpClientFactory.CreateClient();
        var countryResponse = await countryClient.GetAsync("https://localhost:7128/api/Country");
        if (!countryResponse.IsSuccessStatusCode)
        {
            ViewData["ErrorMessage"]="An error occurred while fetching the countries";
            return View();
        }
        var countryContent = await countryResponse.Content.ReadAsStringAsync();
        var countries = JsonConvert.DeserializeObject<ApiResponse<List<CountryGetViewModel>>>(countryContent);
        ViewBag.Countries = countries.Data;


        if (!data.Data.Any())
        {
            return View(new UserViewModel { GetViewModels = new List<UserGetViewModel>() });
        }
        var model = new UserViewModel
        {
            GetViewModels = data.Data
        };
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Update(UserViewModel model)
    {
        var client = httpClientFactory.CreateClient();
        var content = new StringContent(JsonConvert.SerializeObject(model.UpdateViewModel), Encoding.UTF8, "application/json");
        var response = await client.PutAsync("https://localhost:7128/api/Users", content);
        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessageForUpdate"]="An error occurred while updating the user";
            return RedirectToAction("Index");
        }
        TempData["SuccessMessageForUpdate"] = "Kullanıcı başarıyla güncellendi.";
        return RedirectToAction("Index");
    }
}
