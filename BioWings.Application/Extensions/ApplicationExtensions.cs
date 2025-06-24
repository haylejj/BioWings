using BioWings.Application.Interfaces;
using BioWings.Application.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace BioWings.Application.Extensions;
public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(typeof(ApplicationExtensions).Assembly);

        // Authorization Definition Provider servisini kaydet
        services.AddScoped<IAuthorizationDefinitionProvider, AuthorizationDefinitionProvider>();

        return services;
    }
}
