using BioWings.Persistence.Extensions;
using BioWings.WebAPI.Exceptions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
//Add serilog
builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
builder.Services.AddProblemDetails();
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services from the Persistence project
builder.Services.AddPersistenceExtensions(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseExceptionHandler(opt => { });

app.UseAuthorization();

app.MapControllers();

app.Run();
