
using Chocolate.Domain.System;

namespace Chocolate.Application.System.Roles;
public record DeletedRoleCommand(List<long> RoleIds): ICommand;

public class DeletedRoleCommandHandler : ICommandHandler<DeletedRoleCommand>
{
    private readonly IFreeSql freeSql;

    public DeletedRoleCommandHandler(IFreeSql freeSql)
    {
        this.freeSql = freeSql;
    }

    public async Task Handle(DeletedRoleCommand request, CancellationToken cancellationToken)
    {
        var roles = await freeSql.Select<Role>()
            .Where(v => request.RoleIds.Contains(v.Id))
            .ToListAsync();

        await freeSql.Delete<Role>(roles).ExecuteAffrowsAsync(cancellationToken);
        await freeSql.Delete<RoleResource>().Where(v => request.RoleIds.Contains(v.RoleId)).ExecuteAffrowsAsync();
    }
}

