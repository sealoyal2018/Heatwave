using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Chocolate.Application.System.Roles;
public class UpdateRoleCommand : Role, ICommand
{
    public List<long> ResourceIds { get; set; } = [];
}

public class UpdateRoleCommandHandler : ICommandHandler<UpdateRoleCommand>
{
    private readonly IDbAccessor dbAccessor;
    public UpdateRoleCommandHandler(IDbAccessor dbAccessor)
    {
        this.dbAccessor = dbAccessor;
    }

    public async Task Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await dbAccessor.GetIQueryable<Role>().AnyAsync(v => v.Id == request.Id);
        if (!role)
            throw new BusException("未找到相关数据");

        await dbAccessor.UpdateAsync<Role>(
            t => t.Id == request.Id,
            s => s.SetProperty(t => t.Name, request.Name)
                .SetProperty(t => t.Description, request.Description));

        _ = await dbAccessor.DeleteAsync<RoleResource>(v => v.RoleId == request.Id);
        var roleResources = request.ResourceIds.Select(v => new RoleResource
        {
            Id = IdHelper.GetLong(),
            ResourceId = v,
            RoleId = request.Id,
        }).ToList();

        await dbAccessor.InsertAsync(roleResources, false, cancellationToken);
    }
}
