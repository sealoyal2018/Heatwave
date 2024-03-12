using Heatwave.Application.Common;

namespace Heatwave.HostApi.Filters;

public abstract class FilterBase
{
    protected bool IsAjax(HttpRequest request)
    {

        if (request.Headers.TryGetValue("X-Requested-With", out var headerValue))
        {
            return headerValue == "XMLHttpRequest";
        }
        if (request.Headers.Keys.Any(v => v.Contains("-Fetch-")))
        {
            return true;
        }

        return false;
    }

    protected SuccessResult<T> Success<T>(T? data) => new(data);

    protected FailResult Error(string msg, int code = 500) => new FailResult(msg, code);
}
