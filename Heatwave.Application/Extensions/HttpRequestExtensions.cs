using Microsoft.AspNetCore.Http;

using System.Net;
using System.Net.Sockets;

namespace Heatwave.Application.Extensions;
public static class HttpRequestExtensions
{
    /// <summary>
    /// 获取当前请求的IP地址
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static string GetRemoteIpAddress(this HttpRequest request)
    {
        if (request.Headers.ContainsKey("X-Forwarded-For"))
        {
            return request.Headers["X-Forwarded-For"];
        }

        var remoteIP = request.HttpContext.Connection.RemoteIpAddress;
        if(remoteIP.AddressFamily == AddressFamily.InterNetworkV6)
        {
            try
            {
                remoteIP = Dns.GetHostEntry(remoteIP).AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            }
            catch (Exception ex)
            {
                //Log.Error($"获取主机信息失败，主机地址:{remoteIP}");
                //Log.Error(ex, ex.Message);
            }
        }

        return remoteIP.ToString();
    }
}
