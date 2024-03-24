using Heatwave.Application.System.Resources;
using Heatwave.Domain.System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Heatwave.HostApi.Controllers.System;

public class ResourceController : ApiControllerBase
{
    private readonly IMediator mediator;

    public ResourceController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// 获取树形数据
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<dynamic> Tree()
    {
        var res = new ResourceTreeCurrentTenantQuery();
        return await mediator.Send(res);
    }

    [HttpGet]//ICollection<ResourceDigest>
    public async Task<dynamic> List()
    {
        var res = new ResourceListQuery { Name = "" };
        return await mediator.Send(res);
    }

}
