using BioWings.Application.Results;
using BioWings.Domain.Configuration;
using BioWings.UI.Areas.Admin.Models.User;
using BioWings.UI.ViewModels.CountryViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace BioWings.UI.Areas.Admin.Controllers;
[Authorize]
[Area("Admin")]
public class UserController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options) : Controller
{
    private readonly string _baseUrl = options.Value.BaseUrl;
    public async Task<IActionResult> Index()
    {
        var client = httpClientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync($"{_baseUrl}/Users");
        if (!response.IsSuccessStatusCode)
        {
            ViewData["ErrorMessage"]="An error occurred while fetching the data";
            return View();
        }
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ApiResponse<List<UserGetViewModel>>>(content);
        //dropdown list for countries
        var countryClient = httpClientFactory.CreateClient("ApiClient");
        var countryResponse = await countryClient.GetAsync($"{_baseUrl}/Country");
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
        var client = httpClientFactory.CreateClient("ApiClient");
        var content = new StringContent(JsonConvert.SerializeObject(model.UpdateViewModel), Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"{_baseUrl}/Users", content);
        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessageForUpdate"]="An error occurred while updating the user";
            return RedirectToAction("Index");
        }
        TempData["SuccessMessageForUpdate"] = "Kullanıcı başarıyla güncellendi.";
        return RedirectToAction("Index");
    }
}
