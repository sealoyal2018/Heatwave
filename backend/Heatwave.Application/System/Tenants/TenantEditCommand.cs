using Heatwave.Domain.System;

namespace Heatwave.Application.System.Tenants;

/// <summary>
/// 租户编辑
/// </summary>
public class TenantEditCommand : Tenant, ICommand;

public class TenantEditCommandHandler(IDbAccessor dbAccessor): ICommandHandler<TenantEditCommand>
{
    public async Task Handle(TenantEditCommand request, CancellationToken cancellationToken)
    {
        var ret = await dbAccessor.GetIQueryable<Tenant>()
            .AnyAsync(x => x.Id != request.Id && x.Name == request.Name, cancellationToken);

        if (ret)
            throw new BusException("租户名称重复");

        request.Id = IdHelper.GetLong();
        await dbAccessor.InsertAsync<Tenant>(request, cancellation: cancellationToken);
    }
}
