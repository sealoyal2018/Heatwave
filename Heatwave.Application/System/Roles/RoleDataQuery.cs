using AutoMapper;

using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Chocolate.Application.System.Roles;
public record RoleDataQuery(long Id) : IQuery<RoleDisplay>;

public class RoleDataQueryHandler : IQueryHandler<RoleDataQuery, RoleDisplay>
{
    private readonly IMapper mapper;
    private readonly IDbAccessor dbAccessor;

    public RoleDataQueryHandler(IMapper mapper, IDbAccessor dbAccessor)
    {
        this.mapper = mapper;
        this.dbAccessor = dbAccessor;
    }
    public async Task<RoleDisplay> Handle(RoleDataQuery request, CancellationToken cancellationToken)
    {
        var role = await dbAccessor.GetIQueryable<Role>().Where(v => v.Id == request.Id).FirstAsync();
        if (role is null)
            throw new BusException("未找到相关数据", 404);
        var roleResources = await dbAccessor.GetIQueryable<RoleResource>()
            .Include(v => v.Resource)
            .Where(v => v.Id == request.Id)
            .ToListAsync();
        var roleDisplay = mapper.Map<RoleDisplay>(role);
        roleDisplay.Resources = roleResources.Select(v=> v.Resource).ToList();
        return roleDisplay;    
    }
}

public class RoleDisplay: Role, IMapFrom<Role>
{
    public List<Resource> Resources { get; set; } = [];
}