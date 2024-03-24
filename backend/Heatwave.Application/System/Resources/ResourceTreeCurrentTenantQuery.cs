using Heatwave.Domain.System;
using Heatwave.Domain;
using System.Linq.Expressions;

namespace Heatwave.Application.System.Resources;
public record ResourceTreeCurrentTenantQuery : IQuery<ICollection<ResourceTree>>;

public class ResourceTreeCurrentTenantQueryHandler : IQueryHandler<ResourceTreeCurrentTenantQuery, ICollection<ResourceTree>>
{
    private readonly IDbAccessor dbAccessor;

    public ResourceTreeCurrentTenantQueryHandler(IDbAccessor dbAccessor)
    {
        this.dbAccessor = dbAccessor;
    }

    public async Task<ICollection<ResourceTree>> Handle(ResourceTreeCurrentTenantQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Resource, ResourceTree>> selectExpr = r => new ResourceTree { };
        selectExpr = selectExpr.BuildExtendSelectExpre();
        var resources = await dbAccessor.GetIQueryable<Resource>()
            .Select(selectExpr)
            .ToListAsync();

        return resources.Build().ToList();
    }
}
