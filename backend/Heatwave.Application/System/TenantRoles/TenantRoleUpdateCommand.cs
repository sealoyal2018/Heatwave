using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Chocolate.Application.System.Roles;
public class TenantRoleUpdateCommand : TenantRole, ICommand
{
    public List<long> ResourceIds { get; set; } = [];
}

public class TenantRoleUpdateCommandHandler : ICommandHandler<TenantRoleUpdateCommand>
{
    private readonly IDbAccessor dbAccessor;
    public TenantRoleUpdateCommandHandler(IDbAccessor dbAccessor)
    {
        this.dbAccessor = dbAccessor;
    }

    public async Task Handle(TenantRoleUpdateCommand request, CancellationToken cancellationToken)
    {
        var role = await dbAccessor.GetIQueryable<TenantRole>().AnyAsync(v => v.Id == request.Id);
        if (!role)
            throw new BusException("未找到相关数据");

        await dbAccessor.UpdateAsync<TenantRole>(
            t => t.Id == request.Id,
            s => s.SetProperty(t => t.Name, request.Name)
                .SetProperty(t => t.Description, request.Description), cancellation: cancellationToken);

        _ = await dbAccessor.DeleteAsync<TenantRoleResource>(v => v.RoleId == request.Id, cancellationToken);
        var roleResources = request.ResourceIds.Select(v => new TenantRoleResource
        {
            Id = IdHelper.GetLong(),
            ResourceId = v,
            RoleId = request.Id,
            TenantId = request.TenantId
        }).ToList();

        await dbAccessor.InsertAsync(roleResources, false, cancellationToken);
    }
}
