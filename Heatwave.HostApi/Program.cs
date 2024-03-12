using Heatwave.Application;
using Heatwave.Infrastructure;
using Heatwave.HostApi.Extensions;
using Heatwave.HostApi.Filters;
using Heatwave.HostApi.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Heatwave.Application.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(opts =>
{
    opts.Filters.Add<ExceptionFilter>();
    opts.Filters.Add<ResultFilter>();
});
_ = builder.Services.AddSwagger()
    .AddInfrastructure(builder.Configuration)
    .AddApplication(builder.Configuration);

#region jwt хож╓
var jwtConfig = builder.Configuration.GetSection(JwtOption.Name);
builder.Services.Configure<JwtOption>(jwtConfig);
var jwt = jwtConfig.Get<JwtOption>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwt.Issuer,
        ValidAudience = jwt.Audience,
        IssuerSigningKey = jwt.SecurityKey,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
    o.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        { // signalr
            var accessToken = context.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = context.Request.Query["access_token"];
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();
#endregion

#region IdHelper
var idGeneratorSection = builder.Configuration.GetSection(IdGeneratorOption.Name);
IdHelper.Initialization(idGeneratorSection.Get<IdGeneratorOption>());
#endregion

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();
app.MapHubs();

app.Run();
