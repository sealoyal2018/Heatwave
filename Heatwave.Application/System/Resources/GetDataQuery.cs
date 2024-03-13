using Chocolate.Domain.System;

namespace Chocolate.Application.System.Resources;
public record GetDataQuery(long Id) : IQuery<ResourceDisplay>;

public class ResourceDisplay: Resource
{

}

