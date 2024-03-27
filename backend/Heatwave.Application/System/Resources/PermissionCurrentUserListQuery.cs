using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.Resources;

/// <summary>
/// 获取当前登录用户的接口权限
/// </summary>
public record PermissionCurrentUserListQuery : IQuery<ICollection<string>>;

public class PermissionCurrentUserListQueryHandler : IQueryHandler<PermissionCurrentUserListQuery, ICollection<string>>
{
    private readonly ICurrentUser currentUser;

    private readonly IDbAccessor dbAccessor;

    public PermissionCurrentUserListQueryHandler(ICurrentUser currentUser, IDbAccessor dbAccessor)
    {
        this.currentUser = currentUser;
        this.dbAccessor = dbAccessor;
    }
    public async Task<ICollection<string>> Handle(PermissionCurrentUserListQuery request, CancellationToken cancellationToken)
    {
        var userRoles = await dbAccessor.GetIQueryable<TenantUserRole>()
            .Select(v => new TenantUserRole { RoleId = v.RoleId, TenantId = v.TenantId, UserId = v.UserId })
            .Where(v => v.UserId == currentUser.UserId)
            .ToListAsync();
        var roleIds = userRoles.Select(v => v.RoleId).ToList();

        var roleResources = await dbAccessor.GetIQueryable<TenantRoleResource>()
            .Where(v => roleIds.Contains(v.Id))
            .ToListAsync();
        var resourceIds = roleResources.Select(v => v.ResourceId).ToList();
        var resources = await dbAccessor.GetIQueryable<Resource>()
            .Where(v => resourceIds.Contains(v.Id) && v.Type == ResourceType.Request)
            .ToListAsync();
        return resources.Select(v=> v.PermissionCode).ToList();
    }
}
