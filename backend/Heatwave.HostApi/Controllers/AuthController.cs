using System.Text.Json;
using Heatwave.Application.Extensions;
using Heatwave.Application.Interfaces;
using Heatwave.Application.System.Resources;
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
    private readonly IDateTimeService dateTimeService;

    public AuthController(IMediator mediator, CaptchaService captchaService, ICurrentUser currentUser, IDateTimeService dateTimeService)
    {
        this.mediator = mediator;
        this.captchaService = captchaService;
        this.currentUser = currentUser;
        this.dateTimeService = dateTimeService;
    }

    /// <summary>
    /// 获取验证码
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<CaptchaDto> CaptchaAsync()
        => await captchaService.Generate();

    /// <summary>
    /// 查询用户所属租户
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ICollection<TenantDigest>> Tenant([FromQuery]string username)
        => await mediator.Send(new TenantDataByLoginNameQuery(username));

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
        var createUserTokenCommand = new CreateUserTokenCommand(request.TenantId, user.Id, ip);
        var token = await mediator.Send(createUserTokenCommand);

        return new UserTokenDisplay {
            AccessToken = token.Token,
            AccessTokenExpires = token.ExpirationDate,
            RefreshTokenExpires = token.RefreshTokenExpirationDate,
            RefreshToken = token.RefreshToken
        };
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task Logout()
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
    public async Task<UserTokenDisplay> RefreshToken(RefreshTokenCommand refresh)
    {
        var info = await mediator.Send(refresh);
        return new UserTokenDisplay
        {
            AccessToken = info.Token,
            AccessTokenExpires = info.ExpirationDate,
            RefreshTokenExpires = info.RefreshTokenExpirationDate,
            RefreshToken = info.RefreshToken
        };
    }

    /// <summary>
    /// 获取当前登录用户信息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<UserInfo> Info()
    {
        var res = new UserInfo();

        var userDataQuery = new UserDataQuery(currentUser.UserId);
        var user = await mediator.Send(userDataQuery);
        res.NickName = user.NickName;
        res.Email = user.Email;
        res.Phone = user.PhoneNumber;
        var permissionQuery = new PermissionCurrentUserListQuery();
        var resoruces = await mediator.Send(permissionQuery);
        res.Permissions = resoruces;
        return res;
    }

    /// <summary>
    /// 获取当前用户的菜单列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<dynamic> Routes2()
    {
        var json = """
[{
  "path": "/permission",
  "meta": {
    "title": "权限管理",
    "icon": "ep:lollipop",
    "showLink": true,
    "rank": 10
  },
  "children": [
    {
      "path": "/permission/page/index",
      "name": "PermissionPage",
      "meta": {
        "title": "页面权限",
        "showLink": true,
        "roles": ["admin", "common"]
      }
    },
    {
      "path": "/permission/button/index",
      "name": "PermissionButton",
      "meta": {
        "title": "按钮权限",
        "roles": ["admin", "common"],
        "auths": [
          "permission:btn:add",
          "permission:btn:edit",
          "permission:btn:delete"
        ]
      }
    }
  ]
}]
""";
        var data = JsonSerializer.Deserialize<dynamic>(json);
return data;
        // return await mediator.Send(new MenuResourceTreeCurrentUserQuery());

    }

    /// <summary>
    /// 获取当前用户的菜单列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ICollection<MenuResourceTree>> Routes()
    {
        return await mediator.Send(new MenuResourceTreeCurrentUserQuery());
    }
}

public class UserTokenDisplay
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpires { get; set; }
    public DateTime RefreshTokenExpires { get; set; }
}


public class UserInfo
{
    public string NickName { get; set; }
    public string Avatar { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public ICollection<string> Permissions { get; set; } = [];
}