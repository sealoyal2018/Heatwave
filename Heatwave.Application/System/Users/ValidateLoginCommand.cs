using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.Users;

public record ValidateLoginCommand(long TenantId, string UserName, string Password, string Code, string Key): ICommand<User>;

public class ValidateLoginCommandHandler : ICommandHandler<ValidateLoginCommand, User>
{
    private readonly IDbAccessor dbAccessor;

    public ValidateLoginCommandHandler(IDbAccessor dbAccessor)
    {
        this.dbAccessor = dbAccessor;
    }

    public async Task<User> Handle(ValidateLoginCommand request, CancellationToken cancellationToken)
    {
        var user1 = await dbAccessor.GetAllIQueryable<User>()
            .Where(v => v.Email == request.UserName || v.PhoneNumber == request.UserName)
            .FirstOrDefaultAsync();
        var user = await dbAccessor.GetIQueryable<User>()
            .Where(v => v.Email == request.UserName || v.PhoneNumber == request.UserName)
            .FirstOrDefaultAsync();
        if (user is null)
            throw new BusException("账号或者密码错误");
        
        if (string.Compare(user.Password, request.Password.EncodeMD5(), true) != 0)
            throw new BusException("账号或者密码错误");

        return user;
    }
}
