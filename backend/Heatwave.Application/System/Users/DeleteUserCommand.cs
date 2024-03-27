using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.Users;
public record UserDeleteCommand(List<long> Ids): ICommand;

public class UserDeleteCommandHandler(IDbAccessor dbAccessor) : ICommandHandler<UserDeleteCommand>
{
    public async Task Handle(UserDeleteCommand request, CancellationToken cancellationToken)
    {
        await dbAccessor.DeleteAsync<User>(v=> request.Ids.Contains(v.Id), cancellationToken);
    }
}
