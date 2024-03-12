using System.ComponentModel.DataAnnotations;

namespace Heatwave.Domain.System;
public class Role : AuditableEntity
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }
}
