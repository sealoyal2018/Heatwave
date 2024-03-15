using Heatwave.Application.Behaviours;
using Heatwave.Application.Extensions;
using Heatwave.Application.Interfaces;
using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.Users;
public record UserCreateCommand : ICommand
{
    public long TenantId { get; set; }
    public string NickName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public UserType UserType { get; set; } = UserType.Default;

    public ICollection<long> RoleIds { get; set; }
}

public class UserCreateCommandHandler : ICommandHandler<UserCreateCommand>
{
    private readonly IDateTimeService dateTimeService;
    private readonly IDbAccessor dbAccessor;

    public UserCreateCommandHandler(IDbAccessor dbAccessor, IDateTimeService dateTimeService)
    {
        this.dateTimeService = dateTimeService;
        this.dbAccessor = dbAccessor;
    }

    [Transaction]
    public async Task Handle(UserCreateCommand request, CancellationToken cancellationToken)
    {
        var newUser = new User
        {
            Id = IdHelper.GetLong(),
            NickName = request.NickName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Password = request.Password,
        };

        var tenantUser = new TenantUser
        {
            Id = IdHelper.GetLong(),
            TenantId = request.TenantId,
            UserId = newUser.Id,
        };

        if (request.RoleIds.IsNotNullOrEmpty())
        {
            var userRoles = request.RoleIds.Select(v => new TenantUserRole
            {
                Id = IdHelper.GetLong(),
                RoleId = v,
                UserId = newUser.Id,
                TenantId = request.TenantId,
            }).ToList();

            await dbAccessor.InsertAsync(userRoles);
        }
        await dbAccessor.InsertAsync(tenantUser);
        await dbAccessor.InsertAsync(newUser);
    }
}

