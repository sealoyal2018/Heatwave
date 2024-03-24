using Heatwave.Application.Interfaces;
using Heatwave.Domain;
using Heatwave.Domain.System;


namespace Heatwave.Application.System.UserTokens;
public record RefreshTokenCommand(string RefreshToken) : ICommand<UserToken>;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, UserToken>
{
    private readonly IDbAccessor dbAccessor;
    private readonly IDateTimeService dateTimeService;
    private readonly ICurrentUser currentUser;

    public RefreshTokenCommandHandler(IDbAccessor dbAccessor, IDateTimeService dateTimeService, ICurrentUser currentUser)
    {
        this.dbAccessor = dbAccessor;
        this.dateTimeService = dateTimeService;
        this.currentUser = currentUser;
    }

    public async Task<UserToken> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userToken = await this.dbAccessor.GetIQueryable<UserToken>()
            .Include(t => t.User)
            .Where(v => v.RefreshToken == request.RefreshToken && v.UserId == currentUser.UserId)
            .FirstOrDefaultAsync();
        if (userToken is null)
            throw new BusException("refresh token 无效", 401);

        if (dateTimeService.Current() > userToken.RefreshTokenExpirationDate)
            throw new BusException("refresh token 过期", 401);

        // 过期时间在三分钟内刷新才有效
        if (userToken.ExpirationDate < dateTimeService.Current().AddMinutes(-3))
            return userToken;

        var newToken = userToken.GenerateToken();
        var expirationDate = dateTimeService.Current().AddMinutes(10);
        await this.dbAccessor.GetIQueryable<UserToken>()
            .Where(v => v.Id == userToken.Id)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(b => b.Token, newToken)
                    .SetProperty(b => b.TokenHash, newToken.EncodeMD5())
                    .SetProperty(b => b.ExpirationDate, expirationDate)
            );
        return userToken;
    }
}
