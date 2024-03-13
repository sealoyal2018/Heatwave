using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Chocolate.Application.Extensions;
using Chocolate.Domain.System;

namespace Chocolate.Application.System.Roles;
public record RoleDataQuery(long Id) : IQuery<RoleDisplay>;

public class RoleDataQueryHandler : IQueryHandler<RoleDataQuery, RoleDisplay>
{
    private readonly IFreeSql freeSql;
    private readonly IMapper mapper;

    public RoleDataQueryHandler(IFreeSql freeSql, IMapper mapper)
    {
        this.freeSql = freeSql;
        this.mapper = mapper;
    }
    public async Task<RoleDisplay> Handle(RoleDataQuery request, CancellationToken cancellationToken)
    {
        var role = await freeSql.Select<Role>().Where(v => v.Id == request.Id).FirstAsync();
        if (role is null)
            throw new BusException("未找到相关数据", 404);
        var roleResources = await freeSql.Select<RoleResource>()
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