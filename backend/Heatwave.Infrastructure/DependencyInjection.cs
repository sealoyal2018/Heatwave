using System.Reflection;

using Heatwave.Domain;
using Heatwave.Infrastructure.DI;
using Heatwave.Infrastructure.Persistence;
using Heatwave.Infrastructure.Persistence.Extensions;
using Heatwave.Infrastructure.Persistence.Interceptors;
using Heatwave.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Heatwave.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(v => v.FullName.Contains("Heatwave")).ToList();

        services.Scan(tss =>
        {
            tss.FromAssemblies(assemblies)
            .AddClasses(t => t.AssignableTo<IScoped>()).AsSelfWithInterfaces().WithScopedLifetime()
            .AddClasses(t => t.AssignableTo<ITransient>()).AsSelfWithInterfaces().WithTransientLifetime()
            .AddClasses(t => t.AssignableTo<ISingleton>()).AsSelfWithInterfaces().WithSingletonLifetime();
        });


        #region dbcontext
        var databaseOptionSection = configuration.GetRequiredSection(DataBaseOption.Name);
        var databaseOption = databaseOptionSection.Get<DataBaseOption>();

        services.AddDbContext<AppDbContext>((sp, opts) =>
        {
            opts.AddInterceptors(
                sp.GetRequiredService<DispatchDomainEventsInterceptor>(),
                sp.GetRequiredService<AuditableEntityInterceptor>(),
                sp.GetRequiredService<ISoftInterceptor>()
            );
            opts.UseNpgsql(databaseOption.ConnectionString, act =>
            {
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
            });
        });

        services.AddScoped<IDbAccessor, PostgreSqlDbAccessor>();
        #endregion dbcontext

        services.AddScoped<CaptchaService>();

        return services;
    }
}
