using Heatwave.Application.Interfaces;
using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.UserTokens;

public record CreateUserTokenCommand(long TenantId, long UserId, string IpAddress) : ICommand<UserToken>;

public class CreateUserTokenCommandHandler : ICommandHandler<CreateUserTokenCommand, UserToken>
{
    private readonly IDbAccessor dbAccessor;
    private readonly IDateTimeService dateTimeService;
    private readonly IMediator mediator;

    public CreateUserTokenCommandHandler(IDbAccessor dbAccessor, IDateTimeService dateTimeService, IMediator mediator)
    {
        this.dbAccessor = dbAccessor;
        this.dateTimeService = dateTimeService;
        this.mediator = mediator;
    }

    public async Task<UserToken> Handle(CreateUserTokenCommand request, CancellationToken cancellationToken)
    {
        var newUserToken = new UserToken
        {
            Id = IdHelper.GetLong(),
            ExpirationDate = dateTimeService.Current().AddMinutes(30),
            UserId = request.UserId,
            IpAddress = request.IpAddress,
            TenantId = request.TenantId,
            RefreshTokenExpirationDate = dateTimeService.Current().AddMonths(1)
        };
        newUserToken.Token = newUserToken.GenerateToken();
        newUserToken.TokenHash = newUserToken.Token.EncodeMD5();
        newUserToken.RefreshToken = newUserToken.GenerateToken();

        await dbAccessor.InsertAsync(newUserToken);
        return newUserToken;
    }
}
