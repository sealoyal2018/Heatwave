using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

using Heatwave.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Heatwave.Infrastructure.Persistence;
internal abstract class DbAccessorBase : IDbAccessor
{
    private readonly DbContext dbContext;
    protected IDbContextTransaction? _transaction;

    public abstract DbProviderFactory DbProviderFactory { get; }

    public string ConnectionString => dbContext.Database.GetConnectionString();

    public IDbAccessor FullDbAccessor => throw new NotImplementedException();

    public DbAccessorBase(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    protected static List<PropertyInfo> GetKeyPropertys(Type type)
    {
        var properties = type
            .GetProperties()
            .Where(x => x.GetCustomAttributes(true).Select(o => o.GetType().FullName).Contains(typeof(KeyAttribute).FullName))
            .ToList();

        return properties;
    }

    protected abstract string GetSchema(string schema);
    protected abstract string FormatFieldName(string name);
    protected virtual string FormatParamterName(string name)
    {
        return $"@{name}";
    }

    private List<object> GetDeleteList(Type type, List<long> keys)
    {
        var theProperty = GetKeyPropertys(type).FirstOrDefault();
        if (theProperty == null)
            throw new Exception("该实体没有主键标识！请使用[Key]标识主键！");

        List<object> deleteList = new List<object>();
        keys.ForEach(aKey =>
        {
            object newData = Activator.CreateInstance(type);
            var value = Convert.ChangeType(aKey, theProperty.PropertyType);
            theProperty.SetValue(newData, value);
            deleteList.Add(newData);
        });

        return deleteList;
    }

    #region 新增
    public async Task<int> InsertAsync<T>(T entity, bool tracking = false, CancellationToken cancellation = default) where T : class
    {
        await dbContext.Set<T>().AddAsync(entity, cancellation);
        return await dbContext.SaveChangesAsync(cancellation);
    }

    public async Task<int> InsertAsync<T>(List<T> entities, bool tracking = false, CancellationToken cancellation = default) where T : class
    {
        await dbContext.Set<T>().AddRangeAsync(entities, cancellation);
        return await dbContext.SaveChangesAsync(cancellation);
    }
    #endregion

    #region 删除

    public async Task<int> DeleteAsync<T>(long key, CancellationToken cancellation = default) where T : class
    {
        return await DeleteAsync<T>([key], cancellation);
    }

    public Task<int> DeleteAsync<T>(List<long> keys, CancellationToken cancellation = default) where T : class
    {
        var entities = GetDeleteList(typeof(T), keys);
        return DeleteAsync(entities, cancellation);
    }

    public async Task<int> DeleteAllAsync<T>(CancellationToken cancellation = default) where T : class
    {
        await dbContext.Set<T>().ExecuteDeleteAsync(cancellation);
        return await dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync<T>(T entity, CancellationToken cancellation = default) where T : class
    {
        dbContext.Set<T>().Remove(entity);
        return await dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync<T>(List<T> entities, CancellationToken cancellation = default) where T : class
    {
        dbContext.Set<T>().RemoveRange(entities);
        return await dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> condition, CancellationToken cancellation = default) where T : class
    {
        var entities = dbContext.Set<T>().AsNoTracking().AsQueryable();
        var sql = GetDeleteSql(entities);
        return await ExecuteSqlAsync(sql.sql, sql.paramters.ToArray());
    }
    #endregion 删除

    #region 更新

    public async Task<int> UpdateAsync<T>(T entity, bool tracking = false, CancellationToken cancellation = default) where T : class
    {
        dbContext.Entry(entity).State = EntityState.Modified;

        return await dbContext.SaveChangesAsync(cancellation);
    }

    public async Task<int> UpdateAsync<T>(List<T> entities, bool tracking = false, CancellationToken cancellation = default) where T : class
    {
        entities.ForEach(aEntity =>
        {
            dbContext.Entry(aEntity).State = EntityState.Modified;
        });

        return await dbContext.SaveChangesAsync(cancellation);
    }

    public async Task<int> UpdateAsync<T>(T entity, List<string> properties, bool tracking = false, CancellationToken cancellation = default) where T : class
    {
        properties.ForEach(aProperty =>
        {
            dbContext.Entry(entity).Property(aProperty).IsModified = true;
        });
        return await dbContext.SaveChangesAsync(cancellation);
    }

    public async Task<int> UpdateAsync<T>(List<T> entities, List<string> properties, bool tracking = false, CancellationToken cancellation = default) where T : class
    {
        entities.ForEach(aEntity =>
        {
            properties.ForEach(aProperty =>
            {
                dbContext.Entry(aEntity).Property(aProperty).IsModified = true;
            });
        });
        return await dbContext.SaveChangesAsync(cancellation);
    }

    public async Task<int> UpdateAsync<T>(Expression<Func<T, bool>> whereExpre, Action<T> set, bool tracking = false, CancellationToken cancellation = default) where T : class
    {
        var list = await dbContext.Set<T>().AsNoTracking().Where(whereExpre).ToListAsync();

        list.ForEach(aData =>
        {
            set(aData);
        });

        return await UpdateAsync(list, tracking);
    }

    #endregion

    public IQueryable<T> GetIQueryable<T>(bool tracking = false, CancellationToken cancellation = default) where T : class
    {
        if(tracking)
            return dbContext.Set<T>();
        return dbContext.Set<T>().AsNoTracking();
    }

    #region 事务

    public async Task BeginTransactionAsync(IsolationLevel isolationLevel)
    {
        if (_transaction is not null)
            return;

        _transaction = await dbContext.Database.BeginTransactionAsync(isolationLevel);
    }
    public void CommitTransaction()
    {
        if (_transaction is not null)
            _transaction.Commit();
    }
    public void DisposeTransaction()
    {
        if (_transaction is not null)
            _transaction.Dispose();
    }
    public void RollbackTransaction()
    {
        if (_transaction is not null)
            _transaction.Rollback();
    }


    #endregion

    #region 私有方法
    private async Task<int> ExecuteSqlAsync(string sql, params (string paramterName, object paramterValue)[] parameters)
    {
        return await dbContext.Database.ExecuteSqlRawAsync(sql, CreateDbParamters(parameters).ToArray());
    }

    private async Task<int> UpdateSqlAsync(IQueryable source, params (string field, UpdateType updateType, object value)[] values)
    {
        var sql = GetUpdateWhereSql(source, values);

        return await ExecuteSqlAsync(sql.sql, sql.paramters.ToArray());
    }

    private string GetFormatedSchemaAndTableName(Type entityType)
    {
        string fullName = string.Empty;
        string schema = entityType.GetTableSchemaName();
        schema = GetSchema(schema);
        string table = entityType.GetTableName();

        if (string.IsNullOrEmpty(schema))
            fullName = FormatFieldName(table);
        else
            fullName = $"{FormatFieldName(schema)}.{FormatFieldName(table)}";

        return fullName;
    }
    private (string sql, List<(string paramterName, object paramterValue)> paramters) GetWhereSql(IQueryable query)
    {
        List<(string paramterName, object paramterValue)> paramters =
            new List<(string paramterName, object paramterValue)>();
        var querySql = query.ToSql();
        var theSql = querySql.sql.Replace("\r\n", "\n").Replace("\n", " ");

        //替换AS
        var asPattern = "FROM (.*?) AS (.*?) ";
        //倒排防止别名出错
        var asMatchs = Regex.Matches(theSql, asPattern).Cast<Match>().Reverse();
        foreach (Match aMatch in asMatchs)
        {
            var tableName = aMatch.Groups[1].ToString();
            var asName = aMatch.Groups[2].ToString();

            theSql = theSql.Replace(aMatch.Groups[0].ToString(), $"FROM {tableName} ");
            theSql = theSql.Replace(asName + ".", tableName + ".");
        }

        //无筛选
        if (!theSql.Contains("WHERE"))
            return (" 1=1 ", paramters);

        var firstIndex = theSql.IndexOf("WHERE") + 5;
        string whereSql = theSql.Substring(firstIndex, theSql.Length - firstIndex);
        foreach (var aData in querySql.parameters)
        {
            if (whereSql.Contains(aData.Key))
                paramters.Add((aData.Key, aData.Value));
        }

        return (whereSql, paramters);
    }
    private (string sql, List<(string paramterName, object paramterValue)> paramters) GetDeleteSql(IQueryable iq)
    {
        string tableName = GetFormatedSchemaAndTableName(iq.ElementType);
        var whereSql = GetWhereSql(iq);
        string sql = $"DELETE FROM {tableName} WHERE {whereSql.sql}";

        return (sql, whereSql.paramters);
    }
    private (string sql, List<(string paramterName, object paramterValue)> paramters) GetUpdateWhereSql(IQueryable iq, params (string field, UpdateType updateType, object value)[] values)
    {
        string tableName = GetFormatedSchemaAndTableName(iq.ElementType);
        var whereSql = GetWhereSql(iq);

        List<string> propertySetStr = new List<string>();

        values.ToList().ForEach(aProperty =>
        {
            var paramterName = FormatParamterName($"_p_{aProperty.field}");
            string formatedField = FormatFieldName(aProperty.field);
            whereSql.paramters.Add((paramterName, aProperty.value));

            string setValueBody = string.Empty;
            switch (aProperty.updateType)
            {
                case UpdateType.Equal: setValueBody = paramterName; break;
                case UpdateType.Add: setValueBody = $" {formatedField} + {paramterName} "; break;
                case UpdateType.Minus: setValueBody = $" {formatedField} - {paramterName} "; break;
                case UpdateType.Multiply: setValueBody = $" {formatedField} * {paramterName} "; break;
                case UpdateType.Divide: setValueBody = $" {formatedField} / {paramterName} "; break;
                case UpdateType.Concat:
                    {
                        var symbol = "||"; // : "+";
                        setValueBody = $" {formatedField} {symbol} {paramterName} ";
                    }; break;

                default: throw new Exception("updateType无效");
            }

            propertySetStr.Add($" {formatedField} = {setValueBody} ");
        });
        string sql = $"UPDATE {tableName} SET {string.Join(",", propertySetStr)} WHERE {whereSql.sql}";

        return (sql, whereSql.paramters);
    }
    private List<DbParameter> CreateDbParamters((string paramterName, object paramterValue)[] paramters)
    {
        List<DbParameter> dbParamters = new List<DbParameter>();
        foreach (var paramter in paramters)
        {
            var newParamter = DbProviderFactory.CreateParameter();
            newParamter.ParameterName = paramter.paramterName;
            newParamter.Value = paramter.paramterValue;
            dbParamters.Add(newParamter);
        }

        return dbParamters;
    }

    #endregion
}
/// <summary>
/// 更新模式
/// 注:[Field]=[Field] {UpdateType} value
/// </summary>
public enum UpdateType
{
    /// <summary>
    /// 等,即赋值,[Field]= value
    /// </summary>
    Equal = 0,

    /// <summary>
    /// 自增,[Field]=[Field] + value
    /// </summary>
    /// 
    Add = 1,

    /// <summary>
    /// 自减,[Field]=[Field] - value
    /// </summary>
    /// 
    Minus = 2,

    /// <summary>
    /// 自乘,[Field]=[Field] * value
    /// </summary>
    /// 
    Multiply = 3,

    /// <summary>
    /// 自除,[Field]=[Field] / value
    /// </summary>
    Divide = 4,

    /// <summary>
    /// 字符串拼接[Field]=[Field] + value，不同数据库实现有差异
    /// </summary>
    Concat = 5
}

/// <summary>
/// 数据库类型
/// </summary>
public enum DatabaseType
{
    /// <summary>
    /// SqlServer数据库
    /// </summary>
    SqlServer,

    /// <summary>
    /// MySql数据库
    /// </summary>
    MySql,

    /// <summary>
    /// Oracle数据库
    /// </summary>
    Oracle,

    /// <summary>
    /// PostgreSql数据库
    /// </summary>
    PostgreSql,

    /// <summary>
    /// SQLite数据库
    /// </summary>
    SQLite
}