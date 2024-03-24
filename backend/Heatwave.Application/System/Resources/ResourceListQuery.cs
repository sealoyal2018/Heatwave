using AutoMapper;

using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.Resources;

public class ResourceListQuery : IQuery<ICollection<ResourceDigest>>
{
    public string Name { get; set; }
}

public class ResourceDigest : Resource,IMapFrom<Resource>
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
        var resourceIds = new List<long>();
        if (request.Name.IsNotNullOrAny())
        {
            var resources = await dbAccessor.GetIQueryable<Resource>()
                .Select(v => new Resource { Id = v.Id, Title = v.Title })
                .Where(v => v.Title.Contains(request.Name))
                .ToListAsync();
            resourceIds.AddRange(resources.Select(v => v.Id));
        }

        if(currentUser.UserType == UserType.Super)
        {
            var theResources = await dbAccessor.GetIQueryable<Resource>()
                .WhereIf(request.Name.IsNotNullOrAny(), v => resourceIds.Contains(v.Id))
            .ToListAsync();
            return mapper.Map<List<ResourceDigest>>(theResources);
        }
        else
        {
            var tenantResources = await dbAccessor.GetIQueryable<TenantResource>()
                .Include(t => t.Resource)
                .Where(v => v.TenantId == currentUser.TenantId)
                .WhereIf(request.Name.IsNotNullOrAny(), v => resourceIds.Contains(v.ResourceId))
                .ToListAsync();

            resourceIds = tenantResources.Select(v => v.ResourceId).ToList();
            var theResources = await dbAccessor.GetIQueryable<Resource>()
                .Where(v => resourceIds.Contains(v.Id))
                .ToListAsync();
            return mapper.Map<List<ResourceDigest>>(theResources);
        }
    }
}
