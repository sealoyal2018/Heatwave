using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Chocolate.Application.System.TenantRoles;
public record TenantRoleCreateCommand(long TenantId, string Name, string Desc, List<long> ResourceIds) : ICommand;

public class TenantRoleCreateCommandHandle(IDbAccessor dbAccessor) : ICommandHandler<TenantRoleCreateCommand>
{
    public async Task Handle(TenantRoleCreateCommand request, CancellationToken cancellationToken)
    {
        var ret = await dbAccessor.GetIQueryable<TenantRole>()
            .AnyAsync(v => v.TenantId == request.TenantId && v.Name == request.Name, cancellationToken: cancellationToken);
        if (ret)
            throw new BusException("名称重复");

        var newRole = new TenantRole
        {
            Id = IdHelper.GetLong(),
            TenantId = request.TenantId,
            Name = request.Name,
            Description = request.Desc,
        };

        var roleResources = request.ResourceIds.Select(v => new TenantRoleResource
        {
            Id = IdHelper.GetLong(),
            ResourceId = v,
            RoleId = newRole.Id,
            TenantId = request.TenantId,
        }).ToList();

        await dbAccessor.InsertAsync(newRole, cancellation: cancellationToken);
        await dbAccessor.InsertAsync(roleResources, cancellation: cancellationToken);
    }
}
