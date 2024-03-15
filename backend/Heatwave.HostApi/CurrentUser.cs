using System.Security.Claims;

using Heatwave.Application.Extensions;
using Heatwave.Domain;
using Heatwave.Domain.System;
using Heatwave.Infrastructure.DI;

namespace Heatwave.HostApi;

public class CurrentUser : ICurrentUser, IScoped
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public long UserId => this.FindClaimValue<long>(HeatwaveClaimTypes.UserId);

    public string Token => this.FindClaimValue(HeatwaveClaimTypes.Token);

    public UserType UserType => this.FindClaimValue<UserType>(HeatwaveClaimTypes.UserType);

    public bool IsAuthenticated => this.httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public List<long> TenantIds => [];

    public virtual Claim FindClaim(string claimType)
    {
        return this.httpContextAccessor.HttpContext?.User?.FindFirst(claimType);
    }

    public virtual string FindClaimValue(string claimType)
    {
        return FindClaim(claimType)?.Value;
    }

    public virtual T FindClaimValue<T>(string claimType) where T : struct
    {
        var value = FindClaimValue(claimType);

        if (value == null)
        {
            return default;
        }

        return value.To<T>();
    }
}
