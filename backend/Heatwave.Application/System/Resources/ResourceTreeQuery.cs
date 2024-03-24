
using Heatwave.Domain.System;
using Heatwave.Domain;
using System.Linq.Expressions;

namespace Heatwave.Application.System.Resources;

/// <summary>
/// 获取当前用户的菜单、接口权限
/// </summary>
public record ResourceTreeQuery : IQuery<ICollection<ResourceTree>>;

public class ResourceTreeQueryHandler : IQueryHandler<ResourceTreeQuery, ICollection<ResourceTree>>
{
    private readonly ICurrentUser currentUser;
    private readonly IDbAccessor dbAccessor;

    public ResourceTreeQueryHandler(ICurrentUser currentUser, IDbAccessor dbAccessor)
    {
        this.currentUser = currentUser;
        this.dbAccessor = dbAccessor;
    }

    public async Task<ICollection<ResourceTree>> Handle(ResourceTreeQuery request, CancellationToken cancellationToken)
    {
        var userRoles = await dbAccessor.GetIQueryable<TenantUserRole>()
            .Select(v => new TenantUserRole { RoleId = v.RoleId, TenantId = v.TenantId, UserId = v.UserId })
            .Where(v => v.UserId == currentUser.UserId)
            .ToListAsync();
        var roleIds = userRoles.Select(v => v.RoleId).ToList();

        var roleResources = await dbAccessor.GetIQueryable<TenantRoleResource>()
            .Select(v => new TenantRoleResource { ResourceId = v.ResourceId, RoleId = v.RoleId })
            .Where(v => roleIds.Contains(v.Id))
            .ToListAsync();
        var resourceIds = roleResources.Select(v => v.ResourceId).ToList();
        Expression<Func<Resource, ResourceTree>> selectExpr = r => new ResourceTree { };
        selectExpr = selectExpr.BuildExtendSelectExpre();
        var resources = await dbAccessor.GetIQueryable<Resource>()
            .Select(selectExpr)
            .Where(v => resourceIds.Contains(v.Id))
            .ToListAsync();
        return resources.Build().ToList();
    }
}

public class ResourceTree : Resource, INode<ResourceTree>
{
    public long? ParentId => this.ParentId;

    public string Title => this.Title;

    public bool Checked { get; set; }

    public int Rank => base.Rank;

    public ICollection<ResourceTree> Children { get; set; } = [];
}
