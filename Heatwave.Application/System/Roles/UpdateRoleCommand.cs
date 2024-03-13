using Chocolate.Application.Common;
using Chocolate.Domain.System;

namespace Chocolate.Application.System.Roles;
public class UpdateRoleCommand : Role, ICommand
{
    public List<long> ResourceIds { get; set; } = [];
}

public class UpdateRoleCommandHandler : ICommandHandler<UpdateRoleCommand>
{
    private readonly IFreeSql freeSql;
    public UpdateRoleCommandHandler(IFreeSql freeSql)
    {
        this.freeSql = freeSql;
    }

    public async Task Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await freeSql.Select<Role>().AnyAsync(v => v.Id == request.Id);
        if (!role)
            throw new BusException("未找到相关数据");
        await freeSql.Update<Role>(request.Id)
            .Set(v => new Role { Name = request.Name, Description = request.Description })
            .ExecuteAffrowsAsync();

        _ = await freeSql.Delete<RoleResource>()
            .Where(v => v.RoleId == request.Id)
            .ExecuteAffrowsAsync();
        var roleResources = request.ResourceIds.Select(v => new RoleResource
        {
            Id = IdHelper.GetLong(),
            ResourceId = v,
            RoleId = request.Id,
        }).ToList();

        await freeSql.Insert(roleResources).ExecuteAffrowsAsync(cancellationToken);
    }
}
