using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Chocolate.Application.System.Roles;
public record DeletedRoleCommand(List<long> RoleIds): ICommand;

public class DeletedRoleCommandHandler : ICommandHandler<DeletedRoleCommand>
{
    private readonly IDbAccessor dbAccessor;
    public DeletedRoleCommandHandler(IDbAccessor dbAccessor)
    {
        this.dbAccessor = dbAccessor;
    }

    public async Task Handle(DeletedRoleCommand request, CancellationToken cancellationToken)
    {
        var roles = await dbAccessor.GetIQueryable<Role>()
            .Where(v => request.RoleIds.Contains(v.Id))
            .ToListAsync();

        await dbAccessor.DeleteAsync(roles);
        await dbAccessor.DeleteAsync<RoleResource>(v => request.RoleIds.Contains(v.RoleId));
    }
}

