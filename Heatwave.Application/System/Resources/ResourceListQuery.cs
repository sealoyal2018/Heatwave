using Chocolate.Domain.System;

namespace Chocolate.Application.System.Resources;

public record ResourceListQuery: IQuery<IReadOnlyList<Resource>>;

public class ResourceListQueryHandler : IQueryHandler<ResourceListQuery, IReadOnlyList<Resource>>
{
    private readonly IFreeSql freeSql;

    public ResourceListQueryHandler(IFreeSql freeSql)
    {
        this.freeSql = freeSql;
    }

    public async Task<IReadOnlyList<Resource>> Handle(ResourceListQuery request, CancellationToken cancellationToken)
    {
        var resources = await freeSql.GetRepository<Resource>()
            .Select
            .ToListAsync();
        return resources;
    }
}