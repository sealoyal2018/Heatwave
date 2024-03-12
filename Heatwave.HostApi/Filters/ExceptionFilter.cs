using Heatwave.Application.Common;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Heatwave.HostApi.Filters;

public class ExceptionFilter : FilterBase, IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        var ex = context.Exception;
        if (ex is BusException busExp)
        {
            if (IsAjax(context.HttpContext.Request))
            {
                context.Result = new JsonResult(Error(busExp.Message, busExp.Code));
            }
            return Task.CompletedTask;
        }

        context.Result = new JsonResult(Error(ex.ToString(), 500));
        return Task.CompletedTask;
    }
}