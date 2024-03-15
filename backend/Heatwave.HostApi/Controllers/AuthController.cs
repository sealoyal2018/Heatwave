using Heatwave.Application.Extensions;
using Heatwave.Application.Interfaces;
using Heatwave.Application.System.Tenants;
using Heatwave.Application.System.Users;
using Heatwave.Application.System.UserTokens;
using Heatwave.Domain;
using Heatwave.Domain.System;
using Heatwave.Infrastructure.Services;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Heatwave.HostApi.Controllers;

/// <summary>
/// 认证
/// </summary>
public class AuthController : ApiControllerBase
{
    private readonly IMediator mediator;
    private readonly CaptchaService captchaService;
    private readonly ICurrentUser currentUser;

    public AuthController(IMediator mediator, CaptchaService captchaService, ICurrentUser currentUser)
    {
        this.mediator = mediator;
        this.captchaService = captchaService;
        this.currentUser = currentUser;
    }

    /// <summary>
    /// 获取验证码
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<CaptchaDto> CaptcheAsync()
        => await captchaService.Generate();

    [HttpGet]
    [AllowAnonymous]
    public async Task<ICollection<TenantDigest>> Tenant(string username)
        => await mediator.Send(new TenantDataByLoginNameQuery(username));

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost]
    public async Task<UserToken> Token(ValidateLoginCommand request)
    {
        var ret = await captchaService.ValidateAsync(request.Code, request.Key);
        if (ret)
            throw new BusException("验证码不正确");
        var user = await mediator.Send(request);

        var ip = Request.GetRemoteIpAddress();
        var createUserTokenCommand = new CreateUserTokenCommand(user.Id, ip);
        var token = await mediator.Send(createUserTokenCommand);
        return token;
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task SignoutAsync()
    {
        if (!this.currentUser.IsAuthenticated)
        {
            throw new BusException("请先登录");
        }
        var tokenHash = this.currentUser.Token.EncodeMD5();
        // 修改 UserToken 中的 ExpirationDate 为当前时间
        await mediator.Send(new DeletedUesrTokenCommand { TokenHash = tokenHash, UserId = currentUser.UserId });
    }

    /// <summary>
    /// 刷新token
    /// </summary>
    /// <param name="refresh"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<UserToken> RefreshTokenAsync(RefreshTokenCommand refresh)
        => await mediator.Send(refresh);

}

public class UserTokenDisplay
{
    public string Token { get; set; }
}
