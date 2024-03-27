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
    public string PasswordId { get; set; }

    /// <summary>
    /// 电话号码 
    /// </summary>
    public string? PhoneNumber { get; set; }
}