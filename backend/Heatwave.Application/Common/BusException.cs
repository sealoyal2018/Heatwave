namespace Heatwave.Application.Common;
public class BusException(string msg, int code = 500) : Exception(msg)
{
    public int Code { get; } = code;
}
