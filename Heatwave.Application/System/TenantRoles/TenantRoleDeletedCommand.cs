using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Chocolate.Application.System.TenantRoles;
public record TenantRoleDeletedCommand(List<long> RoleIds): ICommand;

public class TenantRoleDeletedCommandHandler : ICommandHandler<TenantRoleDeletedCommand>
{
    private readonly IDbAccessor dbAccessor;
    public TenantRoleDeletedCommandHandler(IDbAccessor dbAccessor)
    {
        this.dbAccessor = dbAccessor;
    }

    public async Task Handle(TenantRoleDeletedCommand request, CancellationToken cancellationToken)
    {
        var roles = await dbAccessor.GetIQueryable<TenantRole>()
            .Where(v => request.RoleIds.Contains(v.Id))
            .ToListAsync();

        await dbAccessor.DeleteAsync(roles);
        await dbAccessor.DeleteAsync<TenantRoleResource>(v => request.RoleIds.Contains(v.RoleId));
    }
}

