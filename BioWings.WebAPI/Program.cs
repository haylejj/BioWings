using BioWings.Application.Extensions;
using BioWings.Domain.Configuration;
using BioWings.Infrastructure.Extensions;
using BioWings.Infrastructure.Hubs;
using BioWings.Persistence.Extensions;
using BioWings.WebAPI.Exceptions;
using BioWings.WebAPI.Extensions;
using BioWings.WebAPI.Filters;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
//Add serilog
builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
builder.Services.AddProblemDetails();
builder.Services.AddSignalR();

// Custom WebAPI Extensions
builder.Services.AddWebApiCors(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<PermissionAuthorizationFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddWebApiSwagger();
builder.Services.AddHttpClient();

// Configure ApiSettings
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Add services from the Application project
builder.Services.AddApplicationExtensions(builder.Configuration);
// Add services from the Persistence project
builder.Services.AddPersistenceExtensions(builder.Configuration);
// Add services from the Infrastructure project
builder.Services.AddInfrastructureExtensions(builder.Configuration);
// global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// jwt configuration
builder.Services.AddWebApiAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowMvcApp");
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseExceptionHandler(opt => { });
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ProgressHub>("/progressHub");
app.MapControllers();

app.Run();
