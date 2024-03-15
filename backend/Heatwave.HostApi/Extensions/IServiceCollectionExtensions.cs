using Asp.Versioning.ApiExplorer;
using Asp.Versioning;
using NSwag.Generation.Processors.Security;
using NSwag;
using System.Net;

namespace Heatwave.HostApi.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddApiVersioning(option =>
        {
            // 可选，为true时API返回支持的版本信息
            option.ReportApiVersions = true;
            // 不提供版本时，默认为1.0
            option.AssumeDefaultVersionWhenUnspecified = true;
            //版本信息放到header ,不写在不配置路由的情况下，版本信息放到response url 中
            option.ApiVersionReader = new HeaderApiVersionReader("api-version");
            // 请求中未指定版本时默认为1.0
            option.DefaultApiVersion = new ApiVersion(1, 0);
        }).AddApiExplorer(option =>
        {          // 版本名的格式：v+版本号
            option.GroupNameFormat = "'v'VVV";
            option.AssumeDefaultVersionWhenUnspecified = true;
        });
        ////获取webapi版本信息，用于swagger多版本支持
        IApiVersionDescriptionProvider provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
        {
            services.AddSwaggerDocument(document =>
            {
                //document.UseControllerSummaryAsTagDescription = true;
                //document.DocumentProcessors.Add(new SwaggerEnumFilter());
                document.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT token"));
                document.DocumentName = description.GroupName;
                document.Version = description.GroupName;
                document.ApiGroupNames = new string[] { description.GroupName };//如果相同版本信息路由下增加
                                                                                //[ApiExplorerSettings(GroupName = "v1")]进行区分即可
                                                                                //jwt 认证
                document.AddSecurity("JWT token", Enumerable.Empty<string>(),
                      new NSwag.OpenApiSecurityScheme()
                      {
                          Type = OpenApiSecuritySchemeType.ApiKey,
                          Name = nameof(Authorization),
                          In = OpenApiSecurityApiKeyLocation.Header,
                          Description = "将token值复制到如下格式: \nBearer {token}"
                      }
                  );
            });
        }
        return services;
    }
}