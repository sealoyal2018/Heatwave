using Heatwave.Application.Behaviours;
using Heatwave.Application.Extensions;
using Heatwave.Application.Interfaces;
using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.Users;
public record CreateUserCommand : ICommand
{
    public string NickName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public UserType UserType { get; set; } = UserType.Default;

    public ICollection<long> RoleIds { get; set; }
}

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    private readonly IDateTimeService dateTimeService;
    private readonly IDbAccessor dbAccessor;

    public CreateUserCommandHandler(IDbAccessor dbAccessor, IDateTimeService dateTimeService)
    {
        this.dateTimeService = dateTimeService;
        this.dbAccessor = dbAccessor;
    }

    [Transaction]
    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var newUser = new User
        {
            Id = IdHelper.GetLong(),
            NickName = request.NickName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Password = request.Password,
            UserType = request.UserType,
        };

        await dbAccessor.InsertAsync(newUser);
        if (request.RoleIds.IsNotNullOrEmpty())
        {
            var userRoles = request.RoleIds.Select(v => new UserRole
            {
                Id = IdHelper.GetLong(),
                RoleId = v,
                UserId = newUser.Id,
            }).ToList();

            throw new BusException("给予错误");
            await dbAccessor.InsertAsync(userRoles);
        }
    }
}

