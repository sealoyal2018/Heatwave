﻿namespace Heatwave.HostApi;

public static class DependencyInjection
{
    public static IServiceCollection AddHostApi(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDistributedMemoryCache();
        return services;
    }
}