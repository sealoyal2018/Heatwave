using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Chocolate.Application.System.Roles;
public record CreateRoleCommand(string Name, string Desc, List<long> ResourceIds) : ICommand;

public class CreateRoleCommandHandle(IDbAccessor dbAccessor) : ICommandHandler<CreateRoleCommand>
{
    private readonly IDbAccessor dbAccessor = dbAccessor;

    public async Task Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var ret = await dbAccessor.GetIQueryable<Role>().AnyAsync(v => v.Name == request.Name);
        if (ret)
            throw new BusException("名称重复");

        var newRole = new Role
        {
            Id = IdHelper.GetLong(),
            Name = request.Name,
            Description = request.Desc,
        };

        var roleResources = request.ResourceIds.Select(v => new RoleResource
        {
            Id = IdHelper.GetLong(),
            ResourceId = v,
            RoleId = newRole.Id
        }).ToList();

        await dbAccessor.InsertAsync(newRole);
        await dbAccessor.InsertAsync(roleResources);
    }
}
