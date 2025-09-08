using System.Reflection;
using GaiaSolution.Application.Base.Interfaces;
using GaiaSolution.Infrastructure.Database;
using GaiaSolution.Infrastructure.Persistence.Interceptors;
using GaiaSolution.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GaiaSolution.Infrastructure.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IClock, Clock>();

        services.AddScoped<AuditSaveChangesInterceptor>();

        services.AddDbContext<CoreDbContext>((sp, options) =>
        {
            options.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        return services;
    }
    
    public static void AddRepositories(this IServiceCollection services)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        assembly.GetTypes().Where(t => $"{assembly.GetName().Name}.Repository" == t.Namespace
                                       && !t.IsAbstract
                                       && !t.IsInterface
                                       && t.Name.EndsWith("Repository"))
            .Select(a => new { assignedType = a, serviceTypes = a.GetInterfaces().ToList() })
            .ToList()
            .ForEach(typesToRegister =>
            {
                typesToRegister.serviceTypes.ForEach(typeToRegister => services.AddScoped(typeToRegister, typesToRegister.assignedType));
            });
    }
}