namespace Heatwave.Domain.System;

/// <summary>
/// 平台
/// </summary>
[Table("sys_app_platform")]
public class AppPlatform : EntityBase, ISoftDeleted
{
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// key
    /// </summary>
    public string? Key { get; set; }
    
    /// <summary>
    /// 密钥
    /// </summary>
    public string? Secret { get; set; }
}