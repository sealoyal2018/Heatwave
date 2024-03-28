namespace Heatwave.Domain;
public interface ISoftDeleted
{
    /// <summary>
    /// 软删除
    /// </summary>
    bool IsDeleted { get; set; }
}
