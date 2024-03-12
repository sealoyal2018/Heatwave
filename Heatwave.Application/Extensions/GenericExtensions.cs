using System.Reflection;

namespace Heatwave.Application.Extensions;
internal static class GenericExtensions
{
    /// <summary>
    /// Dynamically setting object properties and values
    /// </summary>
    /// <param name="this">object</param>
    /// <param name="propertyName">name</param>
    /// <param name="propertyValue">value</param>
    /// <returns></returns>
    public static T? SetPropertyValue<T>(this T @this, string propertyName, object? propertyValue)
    {
        var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (propertyInfo != null)
        {
            //Obtain the real type of itemName
            Type realType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
            if (propertyValue != null)
            {
                propertyValue = Convert.ChangeType(propertyValue, realType);
                propertyInfo.SetValue(@this, propertyValue);
                return @this;
            }
            else
            {
                propertyInfo.SetValue(@this, null);
                return @this;
            }
        }
        return default;
    }
}
