using System.Data.Common;
using System.Linq.Expressions;

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

    /// <summary>
    /// Filters a <see cref="IQueryable{T}"/> by given predicate if given condition is true.
    /// </summary>
    /// <param name="query">Queryable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the query</param>
    /// <returns>Filtered or not filtered query based on <paramref name="condition"/></returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition
            ? query.Where(predicate)
            : query;
    }

    /// <summary>
    /// Filters a <see cref="IQueryable{T}"/> by given predicate if given condition is true.
    /// </summary>
    /// <param name="query">Queryable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the query</param>
    /// <returns>Filtered or not filtered query based on <paramref name="condition"/></returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, int, bool>> predicate)
    {
        return condition
            ? query.Where(predicate)
            : query;
    }

    public static async Task<PaginatedList<T>> ToPageAsync<T,T2>(this IQueryable<T> queryable, T2 paginatedInfo)
        where T2: PaginatedInputBase
    {
        var dict = new Dictionary<string, string>();
        var type = typeof(T2);
        var propertyNames = type.GetProperties().Where(v => v.CanWrite).Select(v => v.Name).ToList();
        var orderQueryable = queryable;
        if (paginatedInfo.Fields.IsNotNullOrEmpty())
        {
            var fields = paginatedInfo.Fields.Split(',');
            var orders = paginatedInfo.Orders.Split(',');

            foreach (var (field, order) in fields.Zip(orders))
            {
                var orderValue = order == "desc" ? "desc" : "aes";
                var propertyName = propertyNames.FirstOrDefault(v => string.Compare(v, field, true) == 0);
                if (propertyName.IsNotNullOrEmpty())
                {
                    if (order == "desc")
                        orderQueryable = orderQueryable.OrderByDescending(t => propertyName);
                    else
                        orderQueryable = orderQueryable.OrderBy(t => propertyName);
                }
            }
        }
        var total = await orderQueryable.LongCountAsync();
        var data = await orderQueryable
            .Skip((paginatedInfo.Index-1) * paginatedInfo.Size)
            .Take(paginatedInfo.Size)
            .ToListAsync();
        return new PaginatedList<T>(data, total, paginatedInfo.Index, paginatedInfo.Size);
    }

}
