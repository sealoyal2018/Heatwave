using FluentValidation;

using Heatwave.Application.Behaviours;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Heatwave.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        services.AddAutoMapper(assembly);
        services.AddValidatorsFromAssembly(assembly);

        // 注意顺序
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        services.AddMediatR(cfg =>
        {
            //cfg.AddBehavior<IRequestPreProcessor<>, ValidationBehaviour>();
            //cfg.AddBehavior(typeof(IRequestPreProcessor<>), typeof(LoggingBehavior<>));
            cfg.RegisterServicesFromAssembly(assembly);
        });
        return services;
    }
}
