using GaiaSolution.Application.Base.Interfaces;
using GaiaSolution.Infrastructure.Base;
using GaiaSolution.Infrastructure.Database;
using GaiaSolution.Infrastructure.Database.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;

namespace GaiaSolution.Infrastructure.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IClock, Clock>();

        services.AddScoped<AuditSaveChangesInterceptor>();

        services.AddDbContext<CoreDbContext>((sp, options) =>
        {
            options.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>()); // option
        });

        return services;
    }
}