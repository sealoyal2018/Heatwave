namespace Heatwave.Domain.System;

public class UserToken : AuditableEntity
{
    /// <summary>
    /// 用户编号
    /// </summary>
    public long UserId { get; set; }
    /// <summary>
    /// Token
    /// </summary>
    public string Token { get; set; }
    /// <summary>
    /// Token Hash 用于查询
    /// </summary>
    public string TokenHash { get; set; }
    /// <summary>
    /// Refresh Token
    /// </summary>
    public string RefreshToken { get; set; }
    /// <summary>
    /// 过期时间
    /// </summary>
    public DateTime ExpirationDate { get; set; }
    /// <summary>
    /// 获取 Token IP 地址
    /// </summary>
    public string IpAddress { get; set; }
    /// <summary>
    /// Refresh Token 是否有效
    /// </summary>
    public bool RefreshTokenIsAvailable { get; set; }

    /// <summary>
    /// Token 所属用户
    /// </summary>
    public virtual User User { get; set; }
}
