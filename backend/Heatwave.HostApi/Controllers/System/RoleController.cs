using Chocolate.Application.System.Roles;
using Chocolate.Application.System.TenantRoles;
using Heatwave.Application.Models;
using Heatwave.Domain.System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Heatwave.HostApi.Controllers.System;

[OpenApiTag("角色")]
public class RoleController(IMediator mediator) : ApiControllerBase
{
    /// <summary>
    /// 获取角色分页
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<PaginatedList<TenantRole>> Page(TenantRolePageListQuery query)
        => await mediator.Send(query);

    /// <summary>
    /// 获取角色详情
    /// </summary>
    /// <param name="id"></param>
    [HttpGet]
    public async Task<TenantRoleDisplay> Data(long id)
        => await mediator.Send(new TenantRoleDataQuery(id));
    
}