using AutoMapper;

using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Chocolate.Application.System.TenantRoles;
public record TenantRoleDataQuery(long Id) : IQuery<TenantRoleDisplay>;

public class TenantRoleDataQueryHandler : IQueryHandler<TenantRoleDataQuery, TenantRoleDisplay>
{
    private readonly IMapper mapper;
    private readonly IDbAccessor dbAccessor;

    public TenantRoleDataQueryHandler(IMapper mapper, IDbAccessor dbAccessor)
    {
        this.mapper = mapper;
        this.dbAccessor = dbAccessor;
    }
    public async Task<TenantRoleDisplay> Handle(TenantRoleDataQuery request, CancellationToken cancellationToken)
    {
        var role = await dbAccessor.GetIQueryable<TenantRole>().Where(v => v.Id == request.Id).FirstAsync();
        if (role is null)
            throw new BusException("未找到相关数据", 404);
        var roleResources = await dbAccessor.GetIQueryable<TenantRoleResource>()
            .Include(v => v.Resource)
            .Where(v => v.Id == request.Id)
            .ToListAsync();
        var roleDisplay = mapper.Map<TenantRoleDisplay>(role);
        roleDisplay.Resources = roleResources.Select(v=> v.Resource).ToList();
        return roleDisplay;    
    }
}

public class TenantRoleDisplay: TenantRole, IMapFrom<TenantRole>
{
    public List<Resource> Resources { get; set; } = [];
}