using Denggaopan.Snowflake;

namespace Heatwave.Application.Common;

public sealed class IdHelper
{
    private static IdGenerator? idGenerator;
    private static object locker = new object();

    public static void Initialization(IdGeneratorOption option)
    {
        if (idGenerator is null)
        {
            lock (locker)
            {
                if (idGenerator is null)
                {
                    idGenerator = new IdGenerator(option.WorkId, option.DataCenterId);
                }
            }
        }
    }

    public static long GetLong()
    {
        return idGenerator?.NextId() ?? throw new TypeInitializationException(typeof(IdHelper).Name, new Exception("idHelper初始化异常"));
    }
}

public class IdGeneratorOption
{
    public static readonly string Name = "IdGenerator";
    public long WorkId { get; set; }
    public long DataCenterId { get; set; }
}

