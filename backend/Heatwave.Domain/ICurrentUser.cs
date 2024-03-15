using Heatwave.Domain.System;

namespace Heatwave.Domain;

public interface ICurrentUser
{
    long UserId { get; }
    UserType UserType { get; }
    string Token { get; }
    bool IsAuthenticated { get; }
    List<long> TenantIds { get; }
}


public static class HeatwaveClaimTypes
{

    public const string UserId = "Heatwave.UserId";
    public const string Email = "Heatwave.Email";
    public const string UserType = "Heatwave.UserType";
    public const string Token = "Heatwave.Token";
}