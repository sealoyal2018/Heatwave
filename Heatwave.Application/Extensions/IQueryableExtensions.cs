using System.Data.Common;

using Microsoft.EntityFrameworkCore;

namespace Heatwave.Application.Extensions;
public static class IQueryableExtensions
{
    /// <summary>
    /// 转为SQL语句，包括参数
    /// </summary>
    /// <param name="query">查询原源</param>
    /// <returns></returns>
    public static (string sql, IReadOnlyDictionary<string, object> parameters) ToSql(this IQueryable query)
    {
        var cmd = query.CreateDbCommand();
        Dictionary<string, object> paramters = new Dictionary<string, object>();
        foreach (DbParameter aCmd in cmd.Parameters)
        {
            paramters.Add(aCmd.ParameterName, aCmd.Value);
        }

        return (cmd.CommandText, paramters);
    }
}
