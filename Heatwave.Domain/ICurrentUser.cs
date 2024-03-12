using Heatwave.Domain.System;

namespace Heatwave.Domain;

public interface ICurrentUser
{
    long UserId { get; set; }
    UserType UserType { get; set; }
}
