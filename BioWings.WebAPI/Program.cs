using BioWings.Application.Extensions;
using BioWings.Infrastructure.Extensions;
using BioWings.Infrastructure.Hubs;
using BioWings.Persistence.Extensions;
using BioWings.WebAPI.Exceptions;
using BioWings.WebAPI.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
//Add serilog
builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
builder.Services.AddProblemDetails();
builder.Services.AddSignalR();
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
builder.Services.AddControllers(options =>
{
    options.Filters.Add<PermissionAuthorizationFilter>();
});
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
// jwt configuration
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
    };
});
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
