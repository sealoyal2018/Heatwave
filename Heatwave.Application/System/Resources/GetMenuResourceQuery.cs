using AutoMapper;

using Chocolate.Application.Extensions;
using Chocolate.Domain.System;

namespace Chocolate.Application.System.Resources;
public record GetMenuResourceQuery : IQuery<ICollection<MenuResource>>;

public class GetMenuResourceQueryHandle : IQueryHandler<GetMenuResourceQuery, ICollection<MenuResource>>
{
    private readonly IFreeSql freeSql;
    private readonly IMapper mapper;

    public GetMenuResourceQueryHandle(IFreeSql freeSql, IMapper mapper)
    {
        this.freeSql = freeSql;
        this.mapper = mapper;
    }

    public async Task<ICollection<MenuResource>> Handle(GetMenuResourceQuery request, CancellationToken cancellationToken)
    {
        var userId = 1643166128945618944;
        var roleIds = await freeSql.GetRepository<UserRole>()
            .Select.Include(v => v.Role)
            .Where(v => v.UserId == userId)
            .Select(v => v.Role.Id)
            .ToListAsync(cancellationToken);

        var resources = await freeSql.GetRepository<RoleResource>()
            .Select.Include(v => v.Resource)
            .Where(v => roleIds.Contains(v.RoleId))
            .ToListAsync(cancellationToken);

        var menuItems = resources.Select(v => v.Resource)
            .Where(v => v.Type != ResourceType.Action)
            .ToList();
        var menus = mapper.Map<List<Resource>, List<MenuResource>>(menuItems);
        return menus.Build();
    }
}

public class MenuResource : Resource, INode<MenuResource>, IMapFrom<Resource>
{
    public long? Pid => this.ParentId;

    public string Text => this.Name;

    public bool Checked { get; set; }

    public int Sort => 0;

    public ICollection<MenuResource> Children { get; set; } = [];
}
