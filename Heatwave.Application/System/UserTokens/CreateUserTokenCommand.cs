using Heatwave.Domain;
using Heatwave.Domain.System;

namespace Heatwave.Application.System.UserTokens;

public class CreateUserTokenCommand: ICommand<UserToken>
{
    public long UserId { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string IpAddress { get; set; }
    public bool RefreshTokenIsAvailable { get; set; }
}

public class CreateUserTokenCommandHandler : ICommandHandler<CreateUserTokenCommand,UserToken>
{
    private readonly IDbAccessor dbAccessor;

    public CreateUserTokenCommandHandler(IDbAccessor dbAccessor)
    {
        this.dbAccessor = dbAccessor;
    }

    public async Task<UserToken> Handle(CreateUserTokenCommand request, CancellationToken cancellationToken)
    {
        var newUserToken = new UserToken
        {
            Id = IdHelper.GetLong(),
            ExpirationDate = request.ExpirationDate,
            UserId = request.UserId,
            Token = request.Token,
            TokenHash = request.Token.EncodeMD5(),
            IpAddress = request.IpAddress,
            RefreshTokenIsAvailable = request.RefreshTokenIsAvailable
        };
        await dbAccessor.InsertAsync(newUserToken);
        return newUserToken;
    }
}
