using System.Linq.Expressions;
using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.Tenants;

/// <summary>
/// 获取租户列表（分页）
/// </summary>
public class TenantPageListQuery : PaginatedInputBase, IQuery<PaginatedList<TenantDigest>>
{
    public string Name { get; set; }
}

public class TenantPageListQueryHandler(IDbAccessor dbAccessor): IQueryHandler<TenantPageListQuery,PaginatedList<TenantDigest>>
{
    public async Task<PaginatedList<TenantDigest>> Handle(TenantPageListQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Tenant, TenantDigest>> selectExpr = t => new TenantDigest { };
        selectExpr = selectExpr.BuildExtendSelectExpre();

        return await dbAccessor.GetIQueryable<Tenant>()
            .Select(selectExpr)
            .WhereIf(request.Name.IsNotEmpty(), v => v.Name.Contains(request.Name))
            .ToPageAsync(request);
    }
}
