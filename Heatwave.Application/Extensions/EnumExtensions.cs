using System.ComponentModel;

namespace Heatwave.Application.Extensions;
internal static class EnumExtensions
{
    public static string GetDescription(this Enum @enum)
    {
        var fi = @enum.GetType().GetField(@enum.ToString());
        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attributes != null && attributes.Length > 0)
            return attributes[0].Description;
        else
            return @enum.ToString();
    }
    public static string GetDescription<TEnum>(this object @object) where TEnum : struct
    {
        return Enum.TryParse(@object.ToString(), out TEnum @enum) ? (@enum as Enum).GetDescription() : @object.ToString();
    }
}
