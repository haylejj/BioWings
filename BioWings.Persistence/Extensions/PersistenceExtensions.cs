using BioWings.Application.Interfaces;
using BioWings.Application.Services;
using BioWings.Persistence.Context;
using BioWings.Persistence.Interceptors;
using BioWings.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BioWings.Persistence.Extensions;
public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistenceExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opt =>
        {
            var connectionString = configuration.GetConnectionString("MySqlConnection") ?? throw new InvalidOperationException("Connection string not found.");
            opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mysqloptions => mysqloptions.EnableRetryOnFailure().MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));
            opt.AddInterceptors(new CustomSaveChangeInterceptor());
        });
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IAuthorityRepository, AuthorityRepository>();
        services.AddScoped<IFamilyRepository, FamilyRepository>();
        services.AddScoped<IGenusRepository, GenusRepository>();
        services.AddScoped<ISpeciesRepository, SpeciesRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IObservationRepository, ObservationRepository>();
        services.AddScoped<IObserverRepository, ObserverRepository>();
        services.AddScoped<IProvinceRepository, ProvinceRepository>();
        services.AddScoped<ISubspeciesRepository, SubspeciesRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        return services;
    }
}