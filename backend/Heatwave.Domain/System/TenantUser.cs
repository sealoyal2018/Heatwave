using System.ComponentModel;

namespace Heatwave.Domain.System;
public class TenantUser : EntityBase, ITenant
{
    public long UserId { get; set; }
    public long TenantId { get; set; }
    /// <summary>
    /// 用户类型
    /// </summary>
    public UserType UserType { get; set; } = UserType.Default;
}

public enum UserType
{
    /// <summary>
    /// 默认用户
    /// </summary>
    [Description("默认用户")]
    Default = 0,

    /// <summary>
    /// 管理员
    /// </summary>
    [Description("管理员")]
    Manager = 1,

    /// <summary>
    /// 超级管理员
    /// </summary>
    [Description("超级管理员")]
    Super = 2,
}
