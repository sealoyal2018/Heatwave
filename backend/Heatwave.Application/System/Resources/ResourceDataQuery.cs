using AutoMapper;

using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.Resources;

public class ResourceDataQuery : IdQueryBase<ResourceDisplay>
{
}

public class ResourceDisplay : Resource, IMapFrom<Resource>
{

}

public class ResourceDataQueryHandler : IQueryHandler<ResourceDataQuery, ResourceDisplay>
{
    private readonly IDbAccessor dbAccessor;
    private readonly IMapper mapper;

    public ResourceDataQueryHandler(IDbAccessor dbAccessor, IMapper mapper)
    {
        this.dbAccessor = dbAccessor;
        this.mapper = mapper;
    }

    public async Task<ResourceDisplay> Handle(ResourceDataQuery request, CancellationToken cancellationToken)
    {
        var resource = await dbAccessor.GetIQueryable<ResourceDisplay>()
         .FirstOrDefaultAsync(v => v.Id == request.Id);
        if (resource is null)
            throw new BusException("未找到相关资源", 404);
        return this.mapper.Map<ResourceDisplay>(resource);
    }
}
