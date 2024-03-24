using Heatwave.Domain;
using Heatwave.Domain.System;
using System.Linq.Expressions;

namespace Heatwave.Application.System.Resources;

/// <summary>
/// 获取当前登录用户的菜单数据
/// </summary>
public record MenuResourceTreeCurrentUserQuery : IQuery<ICollection<MenuResourceTree>>;

public class MenuResourceTreeCurrentUserQueryHandler : IQueryHandler<MenuResourceTreeCurrentUserQuery, ICollection<MenuResourceTree>>
{
    private readonly ICurrentUser currentUser;
    private readonly IDbAccessor dbAccessor;

    public MenuResourceTreeCurrentUserQueryHandler(ICurrentUser currentUser, IDbAccessor dbAccessor)
    {
        this.currentUser = currentUser;
        this.dbAccessor = dbAccessor;
    }

    public async Task<ICollection<MenuResourceTree>> Handle(MenuResourceTreeCurrentUserQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Resource, MenuResourceTree>> selectExpr = r => new MenuResourceTree { };
        selectExpr = selectExpr.BuildExtendSelectExpre();
        if (currentUser.UserType == UserType.Super)
        {
            var superResources = await dbAccessor.GetIQueryable<Resource>()
                .Select(selectExpr)
                .Where(v => v.Type != ResourceType.Action)
                .ToListAsync();
            return superResources.Build().ToList();
        }
        else if (currentUser.UserType == UserType.Manager)
        {
            var tenanetResources = await dbAccessor.GetIQueryable<TenantResource>()
                .Select(v => new TenantResource { ResourceId = v.Id, TenantId = v.TenantId })
                .Where(v => v.TenantId == currentUser.TenantId)
                .ToListAsync();
            var tenantResourceIds = tenanetResources.Select(v => v.TenantId).ToList();
            var superResources = await dbAccessor.GetIQueryable<Resource>()
                .Select(selectExpr)
                .Where(v => tenantResourceIds.Contains(v.Id) && v.Type != ResourceType.Action)
                .ToListAsync();

            return superResources.Build().ToList();
        }

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
        var resources = await dbAccessor.GetIQueryable<Resource>()
            .Select(selectExpr)
            .Where(v => resourceIds.Contains(v.Id) && v.Type != ResourceType.Action)
            .ToListAsync();
        return resources.Build().ToList();
    }
}

public class MenuResourceTree : Resource, INode<MenuResourceTree>
{
    public ICollection<MenuResourceTree> Children { get; set; }

    public MenuMeta Meta
    {
        get
        {
            return new MenuMeta { Icon = Icon, Title = base.Title, Rank = Rank };
        }
    }
}

public class MenuMeta
{
    // 国际化写法
    public string Title { get; set; }
    public string Icon { get; set; }
    public int Rank { get; set; }
    public bool ShowParent { get; set; } = true;
    public bool IsFrame => true;
    public MenuTransition Transition => new MenuTransition("animate__fadeInRight", "animate__fadeOutDown");
}

public record MenuTransition(string EnterTransition, string LeaveTransition);