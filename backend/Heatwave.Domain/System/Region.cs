namespace Heatwave.Domain.System;

/// <summary>
/// 行政区域
/// </summary>
[Table("sys_region")]
public class Region : EntityBase, ISoftDeleted
{
    /// <inheritdoc />
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// 显示名称
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 上级
    /// </summary>
    public long? ParentId { get; set; }
    
    /// <summary>
    /// 排序
    /// </summary>
    public int Rank { get; set; }
}