using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heatwave.Domain.System;

/// <summary>
/// 用户
/// </summary>
[Table("sys_user")]
public class User : AuditableEntity
{
    /// <summary>
    /// 用户名称
    /// </summary>
    public string? NickName { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 电话号码 
    /// </summary>
    public string? PhoneNumber { get; set; }

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
