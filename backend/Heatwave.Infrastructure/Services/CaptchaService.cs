using Heatwave.Infrastructure.DI;

using Microsoft.Extensions.Caching.Distributed;

using SkiaSharp;

namespace Heatwave.Infrastructure.Services;

/// <summary>
/// 验证码服务
/// </summary>
public class CaptchaService : ITransient
{
    private readonly IDistributedCache cache;
    private static readonly string captchaKeyPrefix = "captcha_";
    public CaptchaService(IDistributedCache cache)
    {
        this.cache = cache;
    }

    /// <summary>
    /// 生成验证码
    /// </summary>
    /// <returns></returns>
    public async Task<CaptchaDto> Generate()
    {
        var now = DateTime.Now;
        var captcha = new CaptchaDto();
        var code = CreateCode(4);
        captcha.ExpireTime = now.AddMinutes(1);
        captcha.Captcha = CreateCaptcha(code);
        captcha.Key = Guid.NewGuid().ToString("N");
        var options = new DistributedCacheEntryOptions();
        options.AbsoluteExpiration = new DateTimeOffset(captcha.ExpireTime);
        await this.cache.SetStringAsync($"{captchaKeyPrefix}{captcha.Key}", code, options);
        return captcha;
    }

    /// <summary>
    /// 验证验证码无效
    /// </summary>
    /// <param name="code"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<bool> ValidateAsync(string code, string key)
    {
        var cacheKey = $"{captchaKeyPrefix}{key}";
        var cacheValue = await cache.GetStringAsync(cacheKey);
        if (cacheValue.IsNotNullOrEmpty())
            return false;
        await cache.RemoveAsync(key);
        return string.Compare(cacheValue, code, true) == 0;
    }

    private string CreateCode(int codeLength)
    {
        string so = "1,2,3,4,5,6,7,8,9,0,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
        string[] strArr = so.Split(',');
        string code = "";
        Random rand = new Random();
        for (int i = 0; i < codeLength; i++)
        {
            code += strArr[rand.Next(0, strArr.Length)];
        }
        return code;
    }

    private string CreateCaptcha(string code)
    {
        var width = 128;
        var height = 54;
        Random random = new();
        using var image = new SKBitmap(new SKImageInfo(width, height));
        using var canvas = new SKCanvas(image);
        canvas.DrawColor(SKColor.Parse("#FAE264"));

        //画图片的背景噪音线
        for (int i = 0; i < (width * height * 0.015); i++)
        {
            using SKPaint drawStyle = new();
            drawStyle.Color = new(Convert.ToUInt32(random.Next(Int32.MaxValue)));

            canvas.DrawLine(random.Next(0, width), random.Next(0, height), random.Next(0, width), random.Next(0, height), drawStyle);
        }
        //将文字写到画布上
        using (SKPaint drawStyle = new())
        {
            drawStyle.Color = SKColors.Red;
            drawStyle.TextSize = height;
            drawStyle.StrokeWidth = 1;

            float emHeight = height - (float)height * (float)0.14;
            float emWidth = ((float)width / code.Length) - ((float)width * (float)0.13);

            canvas.DrawText(code, emWidth, emHeight, drawStyle);
        }

        //画图片的前景噪音点
        for (int i = 0; i < (width * height * 0.6); i++)
        {
            image.SetPixel(random.Next(0, width), random.Next(0, height), new SKColor(Convert.ToUInt32(random.Next(Int32.MaxValue))));
        }

        using var img = SKImage.FromBitmap(image);
        using SKData p = img.Encode(SKEncodedImageFormat.Png, 100);
        return Convert.ToBase64String(p.ToArray());
    }


}

public class CaptchaDto
{
    /// <summary>
    /// 验证码(base64字符串)
    /// </summary>
    public string Captcha { get; set; }

    /// <summary>
    /// 验证码 Key
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 验证码过期时间
    /// </summary>
    public DateTime ExpireTime { get; set; }
}
