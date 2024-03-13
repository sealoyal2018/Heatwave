using System.Data;

using Heatwave.Domain;
using Heatwave.Domain.System;

using Microsoft.EntityFrameworkCore;
namespace Heatwave.Application.System.Users;
public class UpdateUserCommand : User, ICommand
{
    public ICollection<long> RoleIds { get; set; }
}

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly IDbAccessor dbAccessor;

    public UpdateUserCommandHandler(IDbAccessor dbAccessor)
    {
        this.dbAccessor = dbAccessor;
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var ret = await dbAccessor.GetIQueryable<User>()
            .AnyAsync(v => v.Id != request.Id && (v.Email == request.Email || v.PhoneNumber == request.PhoneNumber));
        if (ret)
            throw new BusException("邮箱或者手机号码重复");
        _ = await dbAccessor.UpdateAsync<User>(request, [nameof(User.NickName), nameof(User.UserType), nameof(User.Email), nameof(User.PhoneNumber)]);
        await dbAccessor.DeleteAsync<UserRole>(v => v.UserId == request.Id);          
        var userRoles = request.RoleIds.Select(v => new UserRole
        {
            Id = IdHelper.GetLong(),
            RoleId = v,
            UserId = request.Id,
        }).ToList();
        await dbAccessor.InsertAsync(userRoles);
    }
}
