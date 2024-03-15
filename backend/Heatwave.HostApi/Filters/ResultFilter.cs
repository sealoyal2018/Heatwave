using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Heatwave.HostApi.Filters;


public class ResultFilter : FilterBase, IAsyncResultFilter
{
    public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (IsAjax(context.HttpContext.Request))
        {
            if (!context.Filters.Contains(new RawResultAttribute()))
            {
                if (context.Result is ObjectResult obj)
                {
                    context.Result = new JsonResult(Success(obj.Value));
                }
                if (context.Result is EmptyResult)
                {
                    context.Result = new JsonResult(Success("操作成功"));
                }
            }
        }

        return next();
    }
}

public class RawResultAttribute : Attribute, IAsyncActionFilter
{
    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        return next.Invoke();
    }
}
