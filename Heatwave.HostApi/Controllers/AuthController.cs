using Heatwave.Application.Extensions;
using Heatwave.Application.System.Users;
using Heatwave.Application.System.UserTokens;
using Heatwave.Domain;
using Heatwave.Infrastructure.Services;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Heatwave.HostApi.Controllers;

public class AuthController : ApiControllerBase
{
    private readonly IMediator mediator;
    private readonly CaptchaService captchaService;
    private readonly ICurrentUser currentUser;
    private readonly JwtService jwtService;

    public AuthController(IMediator mediator, CaptchaService captchaService, ICurrentUser currentUser, JwtService jwtService)
    {
        this.mediator = mediator;
        this.captchaService = captchaService;
        this.currentUser = currentUser;
        this.jwtService = jwtService;
    }

    /// <summary>
    /// 获取验证码
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<CaptchaDto> CaptcheAsync()
        => await captchaService.Generate();

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost]
    public async Task<UserTokenDisplay> Token(ValidateLoginCommand request)
    {
        var ret = await captchaService.ValidateAsync(request.Code, request.Key);
        if (ret)
            throw new BusException("验证码不正确");
        var user = await mediator.Send(request);

        var ip = Request.GetRemoteIpAddress();

        var token = jwtService.GenerateToken(user);
        
        var createUserTokenCommand = new CreateUserTokenCommand
        {
            UserId = user.Id,
            Token = token,
            RefreshToken = "",
            ExpirationDate = dateTimeService.Current().AddHours(10),
            IpAddress = ip,
            RefreshTokenIsAvailable = true,
        };
        return t;
    }


    /// <summary>
    /// 退出登录
    /// </summary>
    /// <returns></returns>
    [HttpPost("signout")]
    public async Task SignoutAsync()
    {
        if (!this.currentUser.IsAuthenticated)
        {
            throw new BusException("请先登录");
        }
        var tokenHash = this.currentUser.Token.EncodeMD5();
        // 修改 UserToken 中的 ExpirationDate 为当前时间
        var userToken = await userTokenService.GetAsync(a => a.TokenHash == tokenHash && a.UserId == this.CurrentUser.UserId);
        if (userToken != null)
        {
            userToken.ExpirationDate = DateTime.Now;
            await userTokenService.UpdateAsync(userToken);
            // 删除 Redis 中的缓存
            await redisService.DeleteAsync(CoreRedisConstants.UserToken.Format(userToken.TokenHash));
        }

        return Ok();
    }

    public async Task RefreshTokenAsync()
        => await RefreshTokenAsync();


}

public class UserTokenDisplay
{
    public string Token { get; set; }
}
