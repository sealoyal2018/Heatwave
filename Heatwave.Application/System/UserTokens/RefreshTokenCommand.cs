using Heatwave.Application.Interfaces;
using Heatwave.Domain;
using Heatwave.Domain.System;


namespace Heatwave.Application.System.UserTokens;
public record RefreshTokenCommand(long userId, string refreshToken): ICommand<UserToken>;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, UserToken>
{
    private readonly IDbAccessor dbAccessor;
    private readonly IDateTimeService dateTimeService;

    public RefreshTokenCommandHandler(IDbAccessor dbAccessor, IDateTimeService dateTimeService)
    {
        this.dbAccessor = dbAccessor;
        this.dateTimeService = dateTimeService;
    }

    public async Task<UserToken> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userToken = await this.dbAccessor.GetIQueryable<UserToken>()
            .Include(t=> t.User)
            .Where(v => v.RefreshToken == request.refreshToken && v.UserId == request.userId)
            .FirstOrDefaultAsync();
        if (userToken is null)
            throw new BusException("refresh token 无效", 401);

        if(dateTimeService.Current() > userToken.RefreshTokenExpirationDate)
            throw new BusException("refresh token 过期", 401);

        // 过期时间在三分钟内刷新才有效
        if (userToken.ExpirationDate < dateTimeService.Current().AddMinutes(-3))
            return userToken;

        userToken.Token = userToken.GenerateToken();
        userToken.TokenHash = userToken.Token.EncodeMD5();
        userToken.ExpirationDate = dateTimeService.Current().AddMinutes(10);
        await dbAccessor.UpdateAsync(userToken, [nameof(UserToken.Token), nameof(UserToken.TokenHash), nameof(UserToken.ExpirationDate)]);
        return userToken;
    }
}
