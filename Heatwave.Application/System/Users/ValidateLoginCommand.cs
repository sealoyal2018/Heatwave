using Heatwave.Domain;
using Heatwave.Domain.System;

using Microsoft.EntityFrameworkCore;

namespace Heatwave.Application.System.Users;

public record ValidateLoginCommand(string UserName, string Password, string Code, string Key): ICommand<User>;

public class ValidateLoginCommandHandler : ICommandHandler<ValidateLoginCommand, User>
{
    private readonly IDbAccessor dbAccessor;

    public ValidateLoginCommandHandler(IDbAccessor dbAccessor)
    {
        this.dbAccessor = dbAccessor;
    }

    public async Task<User> Handle(ValidateLoginCommand request, CancellationToken cancellationToken)
    {

        var user = await dbAccessor.GetIQueryable<User>()
            .Where(v => v.Email == request.UserName || v.PhoneNumber == request.UserName)
            .FirstOrDefaultAsync();
        if (user is null)
            throw new BusException("账号或者密码错误");
        
        if(user.Password != request.Password)
            throw new BusException("账号或者密码错误");

        return user;
    }
}
