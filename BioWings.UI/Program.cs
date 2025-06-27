using BioWings.Domain.Configuration;
using BioWings.UI.Handler;
using BioWings.UI.Exceptions;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
//For Api consume 
builder.Services.AddHttpClient();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<TokenHandler>();

builder.Services.AddHttpClient("ApiClient")
   .AddHttpMessageHandler<TokenHandler>();

// inject ApiSettings from appsettings.json
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Add services to the container.
builder.Services.AddControllersWithViews();

//global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.SlidingExpiration = true;
        options.LoginPath = "/Login/Login";
        options.LogoutPath = "/Logout/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseExceptionHandler(opt => { });
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
