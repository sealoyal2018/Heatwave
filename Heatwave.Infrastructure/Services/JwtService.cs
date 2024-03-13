using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Heatwave.Application.Interfaces;
using Heatwave.Domain;
using Heatwave.Domain.System;
using Heatwave.Infrastructure.DI;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Heatwave.Infrastructure.Services;
public class JwtService : ITransient
{
    private readonly JwtOption _jwtOption;
    private readonly IDateTimeService dateTimeService;
    public JwtService(IOptions<JwtOption> option, IDateTimeService dateTimeService)
    {
        _jwtOption = option.Value;
        this.dateTimeService = dateTimeService;
    }

    public string GenerateToken(User user)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(HeatwaveClaimTypes.UserId, user.Id.ToString()),
                new Claim(HeatwaveClaimTypes.UserType, user.UserType.ToString()),
                new Claim(HeatwaveClaimTypes.Email, user.Email)
            ]),
            Expires = dateTimeService.Current().AddSeconds(_jwtOption.Expires),
            Issuer = _jwtOption.Issuer,
            Audience = _jwtOption.Audience,
            SigningCredentials = new SigningCredentials(_jwtOption.SecurityKey, SecurityAlgorithms.HmacSha256)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken(User user)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(HeatwaveClaimTypes.UserId, user.Id.ToString()),
                new Claim(HeatwaveClaimTypes.UserType, user.UserType.ToString()),
                new Claim(HeatwaveClaimTypes.Email, user.Email)
            ]),
            Expires = dateTimeService.Current().AddSeconds(_jwtOption.RefreshExpires ?? 1000 * 60 * 60 * 30),
            Issuer = _jwtOption.Issuer,
            Audience = _jwtOption.Audience,
            SigningCredentials = new SigningCredentials(_jwtOption.SecurityKey, SecurityAlgorithms.HmacSha256)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

}
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

    public int? RefreshExpires { get; set; }

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
