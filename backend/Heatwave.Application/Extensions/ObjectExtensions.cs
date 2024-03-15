using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Reflection;

namespace Heatwave.Application.Extensions;
public static class ObjectExtensions
{
    public static string GetTableName(this Type type)
    {
        var tableAttribute = type.GetCustomAttribute<TableAttribute>();
        string tableName;
        if (tableAttribute != null)
            tableName = tableAttribute.Name;
        else
            tableName = type.Name;

        return tableName;
    }

    public static string GetTableSchemaName(this Type type)
    {
        //表名
        var tableAttribute = type.GetCustomAttribute<TableAttribute>();
        return tableAttribute?.Schema;
    }

    /// <summary>
    /// Converts given object to a value type using <see cref="Convert.ChangeType(object,Type)"/> method.
    /// </summary>
    /// <param name="obj">Object to be converted</param>
    /// <typeparam name="T">Type of the target object</typeparam>
    /// <returns>Converted object</returns>
    public static T To<T>(this object obj)
        where T : struct
    {
        if (typeof(T) == typeof(Guid))
        {
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(obj.ToString());
        }

        return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
    }

}
