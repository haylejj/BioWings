using BioWings.Application.Extensions;
using BioWings.Infrastructure.Extensions;
using BioWings.Persistence.Extensions;
using BioWings.WebAPI.Exceptions;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
//Add serilog
builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMvcApp", builder =>
    {
        builder.WithOrigins("https://localhost:7269") // mvc https port
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
// Add services from the Application project
builder.Services.AddApplicationExtensions(builder.Configuration);
// Add services from the Persistence project
builder.Services.AddPersistenceExtensions(builder.Configuration);
// Add services from the Infrastructure project
builder.Services.AddInfrastructureExtensions(builder.Configuration);
// global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
