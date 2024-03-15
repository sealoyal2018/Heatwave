using System.ComponentModel.DataAnnotations.Schema;

namespace Heatwave.Domain.System;

[Table("sys_tenant")]
public class Tenant : AuditableEntity
{
    /// <summary>
    /// 租户名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 授权开始时间
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 授权结束时间
    /// </summary>
    public DateTime EndTime { get; set; }

}
