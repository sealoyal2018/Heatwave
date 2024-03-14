using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Chocolate.Application.System.Roles;
public class RolePageListQuery(string roleName) : PaginatedInputBase, IQuery<PaginatedList<Role>>
{
    public string RoleName { get; } = roleName;
}

public class RolePageListQueryHandler : IQueryHandler<RolePageListQuery, PaginatedList<Role>>
{
    private readonly IDbAccessor dbAccessor;

    public RolePageListQueryHandler(IDbAccessor dbAccessor)
    {
        this.dbAccessor = dbAccessor;
    }

    public async Task<PaginatedList<Role>> Handle(RolePageListQuery request, CancellationToken cancellationToken)
    {
        var d = await dbAccessor.GetIQueryable<Role>()
            .WhereIf(string.IsNullOrEmpty(request.RoleName), v => v.Name.Contains(request.RoleName))
            .ToPageAsync(request);
        return d;
    }
}

