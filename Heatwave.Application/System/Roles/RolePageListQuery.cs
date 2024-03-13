using System.Linq.Expressions;

using Chocolate.Domain.System;

namespace Chocolate.Application.System.Roles;
public class RolePageListQuery(string roleName) : PaginatedInputBase, IQuery<PaginatedList<Role>>
{
    public string RoleName { get; } = roleName;
}

public class RolePageListQueryHandler : IQueryHandler<RolePageListQuery, PaginatedList<Role>>
{
    private readonly IFreeSql freeSql;

    public RolePageListQueryHandler(IFreeSql freeSql)
    {
        this.freeSql = freeSql;
    }

    public async Task<PaginatedList<Role>> Handle(RolePageListQuery request, CancellationToken cancellationToken)
    {
        var d = await freeSql.Select<Role>()
            .WhereIf(string.IsNullOrEmpty(request.RoleName), v => v.Name.Contains(request.RoleName))
            .ToPageAsync(request);
        return d;
    }
}

