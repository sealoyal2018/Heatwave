using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Chocolate.Application.System.Roles;
public class TenantRolePageListQuery(string roleName) : PaginatedInputBase, IQuery<PaginatedList<TenantRole>>
{
    public string RoleName { get; } = roleName;
}

public class RolePageListQueryHandler : IQueryHandler<TenantRolePageListQuery, PaginatedList<TenantRole>>
{
    private readonly IDbAccessor dbAccessor;

    public RolePageListQueryHandler(IDbAccessor dbAccessor)
    {
        this.dbAccessor = dbAccessor;
    }

    public async Task<PaginatedList<TenantRole>> Handle(TenantRolePageListQuery request, CancellationToken cancellationToken)
    {
        var d = await dbAccessor.GetIQueryable<TenantRole>()
            .WhereIf(string.IsNullOrEmpty(request.RoleName), v => v.Name.Contains(request.RoleName))
            .ToPageAsync(request);
        return d;
    }
}

