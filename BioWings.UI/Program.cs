using BioWings.UI.Exceptions;
using BioWings.UI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add HttpClient and ApiSettings
builder.Services.AddUiHttpClient(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();

//global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Add Authentication
builder.Services.AddUiAuthentication();

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
