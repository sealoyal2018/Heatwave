using Heatwave.Domain;
using Heatwave.Infrastructure.DI;
using Heatwave.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Heatwave.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Scan(scan =>
            scan.FromApplicationDependencies()
                .AddClasses(c => c.AssignableTo<ITransient>()).AsImplementedInterfaces().WithTransientLifetime()
                .AddClasses(c => c.AssignableTo<IScoped>()).AsImplementedInterfaces().WithScopedLifetime()
                .AddClasses(c => c.AssignableTo<ISingleton>()).AsImplementedInterfaces().WithSingletonLifetime()
        );

        #region dbcontext
        var databaseOptionSection = configuration.GetRequiredSection(DataBaseOption.Name);
        var databaseOption = databaseOptionSection.Get<DataBaseOption>();
        services.AddDbContext<AppDbContext>(opts =>
        {
            opts.AddInterceptors();
            opts.UseNpgsql(databaseOption.ConnectionString, act =>
            {
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
            });
        });

        services.AddScoped<IDbAccessor, PostgreSqlDbAccessor>();
        #endregion dbcontext

        return services;
    }
}
