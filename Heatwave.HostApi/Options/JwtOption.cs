using Microsoft.IdentityModel.Tokens;

namespace Heatwave.HostApi.Options;

public sealed class JwtOption
{
    public static readonly string Name = "Jwt";

    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Key { get; set; }
    /// <summary>
    /// 秒
    /// </summary>
    public int Expires { get; set; }
    public SymmetricSecurityKey SecurityKey
    {
        get
        {
            var bytes = Encoding.UTF8.GetBytes(Key);
            var securityKey = new SymmetricSecurityKey(bytes);
            return securityKey;
        }
    }
}
