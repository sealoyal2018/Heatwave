namespace Heatwave.Domain;

public class AuditableEntity : EntityBase, ISoftDeleted
{
    /// <summary>
    /// 数据创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; }

    /// <summary>
    /// 数据创建人
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// 数据修改时间
    /// </summary>
    public DateTime? ModifiedTime { get; set; }

    /// <summary>
    /// 数据修改人
    /// </summary>
    public long? ModifiedBy { get; set; }

    /// <summary>
    /// 是否被删除
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 删除者
    /// </summary>
    public long? DeletedBy { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    public DateTime? DeletedTime { get; set; }
}
