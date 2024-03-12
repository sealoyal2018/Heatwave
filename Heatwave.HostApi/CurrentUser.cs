using Heatwave.Domain;
using Heatwave.Domain.System;
using Heatwave.Infrastructure.DI;

namespace Heatwave.HostApi;

public class CurrentUser : ICurrentUser, IScoped
{
    public long UserId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public UserType UserType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
