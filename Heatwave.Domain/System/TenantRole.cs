using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heatwave.Domain.System;

/// <summary>
/// 租户角色
/// </summary>
[Table("sys_tenant_role")]
public class TenantRole : AuditableEntity, ITenant
{
    public long TenantId { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }
}
