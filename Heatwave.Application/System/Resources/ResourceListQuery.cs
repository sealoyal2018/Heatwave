using AutoMapper;

using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.Resources;

public record ResourceListQuery(long TenantId) : IQuery<ICollection<ResourceDigest>>;

public class ResourceDigest : Resource
{
}

public class ResourceListQueryHandler : IQueryHandler<ResourceListQuery, ICollection<ResourceDigest>>
{
    private readonly IDbAccessor dbAccessor;
    private readonly ICurrentUser currentUser;
    private readonly IMapper mapper;
    public ResourceListQueryHandler(IDbAccessor dbAccessor, ICurrentUser currentUser, IMapper mapper)
    {
        this.dbAccessor = dbAccessor;
        this.currentUser = currentUser;
        this.mapper = mapper;
    }


    public async Task<ICollection<ResourceDigest>> Handle(ResourceListQuery request, CancellationToken cancellationToken)
    {
        if (request.TenantId != GlobalContants.AdminTenantId)
        {

        }

        var tenantResources = await dbAccessor.GetIQueryable<TenantResource>()
            .Include(t => t.Resource)
            .Where(v => v.TenantId == request.TenantId)
            .ToListAsync();
        return  mapper.Map<List<ResourceDigest>>(tenantResources.Select(v => v.Resource));
    }
}
