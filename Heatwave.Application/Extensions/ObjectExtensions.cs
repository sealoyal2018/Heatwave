using System.ComponentModel.DataAnnotations.Schema;
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

}
