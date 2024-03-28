using Heatwave.Application.Models;
using Heatwave.Application.System.Tenants;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Heatwave.HostApi.Controllers.System;

/// <summary>
/// 租户
/// </summary>
[OpenApiTag("租户")]
public class TenantController(IMediator mediator) : ApiControllerBase
{
    /// <summary>
    /// 租户分页
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<PaginatedList<TenantDigest>> Page(TenantPageListQuery query)
        => await mediator.Send(query);
}