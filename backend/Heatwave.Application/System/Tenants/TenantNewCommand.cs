using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.Tenants;

/// <summary>
/// 创建租户
/// </summary>
public class TenantNewCommand : Tenant, ICommand;

public class TenantNewCommandHandler(IDbAccessor dbAccessor): ICommandHandler<TenantNewCommand>
{
    public async Task Handle(TenantNewCommand request, CancellationToken cancellationToken)
    {
        var ret = await dbAccessor.GetIQueryable<Tenant>()
            .AnyAsync(x => x.Name == request.Name, cancellationToken);

        if (ret)
            throw new BusException("租户名称重复");

        request.Id = IdHelper.GetLong();
        await dbAccessor.InsertAsync<Tenant>(request, cancellation: cancellationToken);
    }
}
