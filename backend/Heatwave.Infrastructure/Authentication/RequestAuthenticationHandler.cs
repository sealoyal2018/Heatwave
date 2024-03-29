﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using AutoMapper;

using Heatwave.Application.Interfaces;
using Heatwave.Application.System.Users;
using Heatwave.Application.System.UserTokens;
using Heatwave.Domain;
using Heatwave.Domain.System;
using MediatR;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Heatwave.Infrastructure.Authentication;
public class RequestAuthenticationHandler : AuthenticationHandler<RequestAuthenticationSchemeOptions>
{
    private readonly IMediator mediator;
    private readonly IDateTimeService dateTimeService;

    public RequestAuthenticationHandler(
        IOptionsMonitor<RequestAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IMediator mediator,
        IDateTimeService dateTimeService) : base(options, logger, encoder, clock)
    {
        this.mediator = mediator;
        this.dateTimeService = dateTimeService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var token = Request.Headers.Authorization.ToString();

        if (!string.IsNullOrEmpty(token))
        {
            token = token.Trim();

            var userToken = await this.mediator.Send(new UserTokenDataQuery { TokenHash = token.EncodeMD5() });
            // 验证 Token 是否有效，并获取用户信息
            if (userToken is null)
            {
                return AuthenticateResult.Fail("Invalid Token!");
            }

            if(userToken.ExpirationDate.AddMinutes(2) < dateTimeService.Current())
                return AuthenticateResult.Fail("Invalid Token!");

            //var user = await this.mediator.Send(new TenantUser(userToken.UserId));


            var claims = new List<Claim>
            {
                new(HeatwaveClaimTypes.UserId, userToken.UserId.ToString()),
                new(HeatwaveClaimTypes.Token, token),
                new(HeatwaveClaimTypes.UserType, "2"),
                new(ClaimTypes.NameIdentifier, userToken.UserId.ToString()),
                new(HeatwaveClaimTypes.TenantId, userToken.TenantId.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(claims, nameof(RequestAuthenticationHandler));

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), this.Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
        return AuthenticateResult.NoResult();
    }
}


public class RequestAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public static readonly string SchemeName = "Authorization-Token";
}