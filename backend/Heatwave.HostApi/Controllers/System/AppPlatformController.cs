using Heatwave.Application.Models;
using Heatwave.Application.System.AppPlatforms;
using Heatwave.Domain.System;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Heatwave.HostApi.Controllers.System;

public class AppPlatformController(IMediator mediator) : ApiControllerBase
{
    [HttpPost]
    public async Task<PaginatedList<AppPlatformDigest>> Page(AppPlatformPageListQuery query)
        => await mediator.Send(query);
}