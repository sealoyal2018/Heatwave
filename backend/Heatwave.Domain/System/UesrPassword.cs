namespace Heatwave.Domain.System;

[Table("sys_user_password")]
public class UesrPassword : EntityBase
{
    public long UserId { get; set; }

    public string Password { get; set; }
    
    public EncryptType EncryptType { get; set; }
    
    private UesrPassword(){}

    public UesrPassword(long userId, string password, EncryptType type)
    {
        Id = IdHelper.GetLong();
        this.UserId = userId;
        this.Password = password;
        this.EncryptType = type;
    }
}

public enum EncryptType
{
    /// <summary>
    /// md5加密
    /// </summary>
    MD5 = 0,
    /// <summary>
    /// aes加密
    /// </summary>
    AES = 1,
    /// <summary>
    /// des 加密
    /// </summary>
    DES = 2,
}
