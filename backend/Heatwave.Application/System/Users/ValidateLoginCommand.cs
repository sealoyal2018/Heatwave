using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.Users;

public class ValidateLoginCommand: ICommand<User>
{
    public long TenantId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Code { get; set; }
    public string Key { get; set; }
}

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
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (user is null)
            throw new BusException("账号或者密码错误");

        var userPassword = await dbAccessor.GetIQueryable<UesrPassword>()
            .Where(v => v.UserId == user.Id)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (userPassword is null)
            throw new BusException("账号或者密码错误");
        
        if (string.Compare(userPassword.Password, request.Password.EncodeMD5(), StringComparison.OrdinalIgnoreCase) != 0)
            throw new BusException("账号或者密码错误");

        return user;
    }
}
