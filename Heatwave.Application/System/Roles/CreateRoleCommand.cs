using Chocolate.Application.Common;
using Chocolate.Domain.System;

namespace Chocolate.Application.System.Roles;
public record CreateRoleCommand(string Name, string Desc, List<long> ResourceIds) : ICommand;

public class CreateRoleCommandHandle(IFreeSql freeSql) : ICommandHandler<CreateRoleCommand>
{
    private readonly IFreeSql freeSql = freeSql;

    public async Task Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var ret = await freeSql.Select<Role>().AnyAsync(v => v.Name == request.Name);
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

        await freeSql.Insert(newRole).ExecuteInsertedAsync(cancellationToken);
        await freeSql.Insert(roleResources).ExecuteInsertedAsync(cancellationToken);
    }
}
