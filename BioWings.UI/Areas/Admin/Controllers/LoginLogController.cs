using BioWings.Application.DTOs.LoginLogDtos;
using BioWings.Application.Results;
using BioWings.Domain.Configuration;
using BioWings.UI.Areas.Admin.Models.LoginLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BioWings.UI.Areas.Admin.Controllers;

[Authorize]
[Area("Admin")]
public class LoginLogController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options) : Controller
{
    private readonly string _baseUrl = options.Value.BaseUrl;

    public async Task<IActionResult> Index()
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"{_baseUrl}/LoginLog/recent?count=100");

            if (!response.IsSuccessStatusCode)
            {
                ViewData["ErrorMessage"] = "Giriş logları yüklenirken bir hata oluştu.";
                return View(new LoginLogIndexViewModel());
            }

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<LoginLogCreateDto>>>(content);

            var viewModel = new LoginLogIndexViewModel();

            if (apiResponse?.IsSuccess == true && apiResponse.Data != null)
            {
                viewModel.LoginLogs = apiResponse.Data.Select(log => new LoginLogViewModel
                {
                    Id = log.Id,
                    UserId = log.UserId,
                    UserName = log.UserName,
                    IpAddress = log.IpAddress,
                    LoginDateTime = log.LoginDateTime,
                    UserAgent = log.UserAgent,
                    IsSuccessful = log.IsSuccessful,
                    FailureReason = log.FailureReason
                }).OrderByDescending(x => x.LoginDateTime).ToList();

                viewModel.TotalCount = viewModel.LoginLogs.Count;
                viewModel.SuccessfulCount = viewModel.LoginLogs.Count(x => x.IsSuccessful);
                viewModel.FailedCount = viewModel.LoginLogs.Count(x => !x.IsSuccessful);
            }
            else if (apiResponse?.IsSuccess == false && apiResponse.ErrorList != null)
            {
                ViewData["ErrorMessage"] = string.Join(", ", apiResponse.ErrorList);
            }

            return View(viewModel);
        }
        catch (Exception ex)
        {
            ViewData["ErrorMessage"] = $"Bir hata oluştu: {ex.Message}";
            return View(new LoginLogIndexViewModel());
        }
    }

    public async Task<IActionResult> UserHistory(int userId)
    {
        try
        {
            var client = httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"{_baseUrl}/LoginLog/user/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                ViewData["ErrorMessage"] = "Kullanıcı giriş geçmişi yüklenirken bir hata oluştu.";
                return View(new LoginLogIndexViewModel());
            }

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<LoginLogCreateDto>>>(content);

            var viewModel = new LoginLogIndexViewModel();

            if (apiResponse?.IsSuccess == true && apiResponse.Data != null)
            {
                viewModel.LoginLogs = apiResponse.Data.Select(log => new LoginLogViewModel
                {
                    Id = log.Id,
                    UserId = log.UserId,
                    UserName = log.UserName,
                    IpAddress = log.IpAddress,
                    LoginDateTime = log.LoginDateTime,
                    UserAgent = log.UserAgent,
                    IsSuccessful = log.IsSuccessful,
                    FailureReason = log.FailureReason
                }).OrderByDescending(x => x.LoginDateTime).ToList();

                viewModel.TotalCount = viewModel.LoginLogs.Count;
                viewModel.SuccessfulCount = viewModel.LoginLogs.Count(x => x.IsSuccessful);
                viewModel.FailedCount = viewModel.LoginLogs.Count(x => !x.IsSuccessful);
            }
            else if (apiResponse?.IsSuccess == false && apiResponse.ErrorList != null)
            {
                ViewData["ErrorMessage"] = string.Join(", ", apiResponse.ErrorList);
            }

            ViewBag.UserId = userId;
            ViewBag.PageTitle = $"Kullanıcı ID: {userId} - Giriş Geçmişi";

            return View("Index", viewModel);
        }
        catch (Exception ex)
        {
            ViewData["ErrorMessage"] = $"Bir hata oluştu: {ex.Message}";
            return View("Index", new LoginLogIndexViewModel());
        }
    }
}