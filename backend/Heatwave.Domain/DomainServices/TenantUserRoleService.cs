using Heatwave.Domain.System;

using Microsoft.EntityFrameworkCore;

namespace Heatwave.Domain.DomainServices;

public class TenantUserRoleService : IDomainService
{
    private readonly IDbAccessor dbAccessor;

    public TenantUserRoleService(IDbAccessor dbAccessor)
    {
        this.dbAccessor = dbAccessor;
    }

    public async Task<ICollection<TenantRole>> GetUserRoles(long tenantId, long userId)
    {
        var userRoles = await dbAccessor.GetIQueryable<TenantUserRole>()
            .Where(v => v.TenantId == tenantId && v.UserId == userId)
            .ToListAsync();
        var roleIds = userRoles.Select(v => v.Id).ToList();
        return await dbAccessor.GetIQueryable<TenantRole>()
            .Where(v => roleIds.Contains(v.Id))
            .ToListAsync();
    }

}
