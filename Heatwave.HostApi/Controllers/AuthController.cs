using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Heatwave.HostApi.Controllers;

public class AuthController : ApiControllerBase
{

    [AllowAnonymous]
    [HttpGet]
    public async Task<UserTokenDisplay> Token()
    {
        var t = new UserTokenDisplay();
        return t;
    }
}

public class UserTokenDisplay
{
    public string Token { get; set; }
}
