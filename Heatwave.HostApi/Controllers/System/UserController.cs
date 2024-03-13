using Heatwave.Application.System.Users;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Heatwave.HostApi.Controllers.System;

public class UserController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task CreateAsync(CreateUserCommand request)
        => await _mediator.Send(request);

}
