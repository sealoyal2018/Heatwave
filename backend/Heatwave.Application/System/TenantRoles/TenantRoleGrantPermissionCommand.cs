using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.TenantRoles;

/// <summary>
/// 角色授权
/// </summary>
/// <param name="TenantRoleId"></param>
/// <param name="ResourceIds"></param>
public record TenantRoleGrantPermissionCommand(long TenantRoleId, List<long> ResourceIds) : ICommand;

public class TenantRoleGrantPermissionCommandHandler : ICommandHandler<TenantRoleGrantPermissionCommand>
{
    private readonly IDbAccessor _dbAccessor;

    public TenantRoleGrantPermissionCommandHandler(IDbAccessor dbAccessor)
    {
        _dbAccessor = dbAccessor;
    }

    public async Task Handle(TenantRoleGrantPermissionCommand request, CancellationToken cancellationToken)
    {
        var tenantRoleResources = request.ResourceIds.Select(v => new TenantRoleResource
        {
            Id = IdHelper.GetLong(),
            ResourceId = v,
            RoleId = request.TenantRoleId,
        }).ToList();
        await _dbAccessor.InsertAsync(tenantRoleResources, cancellation: cancellationToken);
    }
}

