using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace Heatwave.Domain.System;

[Table("sys_user_token")]
public class UserToken : EntityBase
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
    /// token 过期时间
    /// </summary>
    public DateTime ExpirationDate { get; set; }
    /// <summary>
    /// 获取 Token IP 地址
    /// </summary>
    public string IpAddress { get; set; }
    /// <summary>
    /// Refresh Token 过期时间
    /// </summary>
    public DateTime RefreshTokenExpirationDate { get; set; }

    /// <summary>
    /// Token 所属用户
    /// </summary>
    public virtual User User { get; set; }

    /// <summary>
    /// 生成token
    /// </summary>
    /// <returns></returns>
    public string GenerateToken()
    {
        using var md5 = MD5.Create();
        var key = IdHelper.GetLong();
        var str = $"{key}-{this.UserId}";
        var bytes = Encoding.UTF8.GetBytes(str);
        var md5Array = md5.ComputeHash(bytes);
        return Convert.ToBase64String(md5Array);
    }
}
