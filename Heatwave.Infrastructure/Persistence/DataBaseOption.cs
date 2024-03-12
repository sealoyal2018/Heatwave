namespace Heatwave.Infrastructure.Persistence;
public class DataBaseOption
{
    public static string Name = "DataBase";

    /// <summary>
    /// 类型
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// 字符串
    /// </summary>
    public string ConnectionString { get; set; }
}
