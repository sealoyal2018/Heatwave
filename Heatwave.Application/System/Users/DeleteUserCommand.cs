using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.Users;
public record DeleteUserCommand(List<long> Ids): ICommand;

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly IDbAccessor dbAccessor;

    public DeleteUserCommandHandler(IDbAccessor dbAccessor)
    {
        this.dbAccessor = dbAccessor;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        await dbAccessor.DeleteAsync<User>(v=> request.Ids.Contains(v.Id));
    }
}
