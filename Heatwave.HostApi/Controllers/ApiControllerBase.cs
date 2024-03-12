using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Heatwave.HostApi.Controllers;

[ApiController, Authorize, Route("[controller]/[action]")]
public abstract class ApiControllerBase : ControllerBase
{
}
