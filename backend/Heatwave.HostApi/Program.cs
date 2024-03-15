using Heatwave.Application;
using Heatwave.Infrastructure;
using Heatwave.HostApi.Extensions;
using Heatwave.HostApi.Filters;
using Heatwave.Infrastructure.Authentication;
using Heatwave.Domain;
using Heatwave.HostApi;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        #region IdHelper
        var idGeneratorSection = builder.Configuration.GetSection(IdGeneratorOption.Name);
        IdHelper.Initialization(idGeneratorSection.Get<IdGeneratorOption>());
        #endregion

        // Add services to the container.
        builder.Services.AddDistributedMemoryCache();

        _ = builder.Services.AddSwagger()
            .AddInfrastructure(builder.Configuration)
            .AddApplication(builder.Configuration)
            .AddHostApi(builder.Configuration);

        builder.Services.AddAuthentication(RequestAuthenticationSchemeOptions.SchemeName)
            .AddScheme<RequestAuthenticationSchemeOptions, RequestAuthenticationHandler>(RequestAuthenticationSchemeOptions.SchemeName, opts => { });

        builder.Services.AddControllers(opts =>
        {
            opts.Filters.Add<ExceptionFilter>();
            opts.Filters.Add<ResultFilter>();
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
        }

        app.UseCors(opts =>
        {
            opts.SetIsOriginAllowed(_ => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAuthorization();

        app.MapControllers().RequireAuthorization();
        app.MapHubs();
        app.Run();
    }
}

#region IdHelper

#endregion
