using Heatwave.Application.Interfaces;
using Heatwave.Infrastructure.DI;

namespace Heatwave.Infrastructure.Services;
internal class DateTimeService : IDateTimeService, IScoped
{
    private readonly DateTime current;

    public DateTimeService()
    {
        current = DateTime.Now;
    }
    public DateTime Current() => current;
}
