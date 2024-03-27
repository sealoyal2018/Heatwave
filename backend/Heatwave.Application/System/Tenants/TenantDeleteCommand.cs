using Heatwave.Domain.System;

namespace Heatwave.Application.System.Tenants;

/// <summary>
/// 删除租户
/// </summary>
/// <param name="Id"></param>
public record TenantDeleteCommand(params long[] Ids) : ICommand;

public class TenantDeleteCommandHandler(IDbAccessor dbAccessor): ICommandHandler<TenantDeleteCommand>
{
    public async Task Handle(TenantDeleteCommand request, CancellationToken cancellationToken)
    {
        await dbAccessor.DeleteAsync<Tenant>(request.Ids, cancellationToken);
    }
}
