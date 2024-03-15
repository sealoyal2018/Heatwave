namespace Heatwave.HostApi.Extensions;

public static class IEndpointBuilderExtensions
{
    public static IEndpointRouteBuilder MapHubs(this IEndpointRouteBuilder app)
    {
        return app;
    }

    public static IApplicationBuilder UseSwagger(this IApplicationBuilder app)
    {
        app.UseOpenApi().UseSwaggerUi(setting => { });
        return app;
    }
}
