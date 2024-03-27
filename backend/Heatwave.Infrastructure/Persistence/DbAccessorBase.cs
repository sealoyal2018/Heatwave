using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Heatwave.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Query;

namespace Heatwave.Infrastructure.Persistence;

internal abstract class DbAccessorBase(AppDbContext dbContext) : IDbAccessor
{
    private readonly DbContext dbContext = dbContext;
    protected IDbContextTransaction? _transaction;

    public abstract DbProviderFactory DbProviderFactory { get; }

    public string ConnectionString => dbContext.Database.GetConnectionString();

    #region 新增
    public async Task<int> InsertAsync<T>(T entity, bool tracking = false, CancellationToken cancellation = default) where T : class
    {
        await dbContext.Set<T>().AddAsync(entity, cancellation);
        return await dbContext.SaveChangesAsync(cancellation);
    }

    public async Task<int> InsertAsync<T>(ICollection<T> entities, bool tracking = false, CancellationToken cancellation = default) where T : class
    {
        await dbContext.Set<T>().AddRangeAsync(entities, cancellation);
        return await dbContext.SaveChangesAsync(cancellation);
    }
    #endregion

    #region 删除

    public async Task<int> DeleteAsync<T>(long key, CancellationToken cancellation = default) where T : EntityBase
    {
        return await DeleteAsync<T>([key], cancellation);
    }

    public async Task<int> DeleteAsync<T>(ICollection<long> keys, CancellationToken cancellation = default) where T : EntityBase
    {
        return await dbContext.Set<T>().Where(v => keys.Contains(v.Id)).ExecuteDeleteAsync(cancellationToken: cancellation);
    }

    public async Task<int> DeleteAllAsync<T>(CancellationToken cancellation = default) where T : class
    {
        await dbContext.Set<T>().ExecuteDeleteAsync(cancellation);
        return await dbContext.SaveChangesAsync(cancellation);
    }

    public async Task<int> DeleteAsync<T>(T entity, CancellationToken cancellation = default) where T : class
    {
        dbContext.Set<T>().Remove(entity);
        return await dbContext.SaveChangesAsync(cancellation);
    }

    public async Task<int> DeleteAsync<T>(ICollection<T> entities, CancellationToken cancellation = default) where T : class
    {
        dbContext.Set<T>().RemoveRange(entities);
        return await dbContext.SaveChangesAsync(cancellation);
    }

    public async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> condition, CancellationToken cancellation = default) where T : class
    {
        return await dbContext.Set<T>().Where(condition).ExecuteDeleteAsync(cancellationToken: cancellation);
    }
    #endregion 删除

    #region 更新

    public async Task<int> UpdateAsync<T>(T entity, bool tracking = false, CancellationToken cancellation = default) where T : class
    {
        dbContext.Entry(entity).State = EntityState.Modified;

        return await dbContext.SaveChangesAsync(cancellation);
    }

    public async Task<int> UpdateAsync<T>(ICollection<T> entities, bool tracking = false, CancellationToken cancellation = default) where T : class
    {
        foreach (var entity in entities)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
        }

        return await dbContext.SaveChangesAsync(cancellation);
    }

    public async Task<int> UpdateAsync<T>(T entity, ICollection<string> properties, bool tracking = false, CancellationToken cancellation = default) where T : class
    {
        foreach (var property in properties)
        {
            dbContext.Entry(entity).Property(property).IsModified = true;
        }

        return await dbContext.SaveChangesAsync(cancellation);
    }

    public async Task<int> UpdateAsync<T>(ICollection<T> entities, ICollection<string> properties, bool tracking = false, CancellationToken cancellation = default) where T : class
    {
        foreach (var entity in entities)
        {
            foreach (var property in properties)
            {
                dbContext.Entry(entity).Property(property).IsModified = true;
            }
        }
        return await dbContext.SaveChangesAsync(cancellation);
    }

    public async Task<int> UpdateAsync<T>(Expression<Func<T, bool>> whereExpr, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> action, bool tracking = false, CancellationToken cancellation = default) where T : class
    {
        return await dbContext.Set<T>().Where(whereExpr).ExecuteUpdateAsync(action, cancellationToken: cancellation);
    }

    #endregion

    public IQueryable<T> GetIQueryable<T>(bool tracking = false, CancellationToken cancellation = default) where T : class
    {
        var queryable = (tracking) switch
        {
            true => dbContext.Set<T>(),
            _ => dbContext.Set<T>().AsNoTracking()
        };

        if (typeof(T).IsAssignableTo(typeof(ISoftDeleted)))
        {
            queryable = queryable.Where($"{nameof(ISoftDeleted.IsDeleted)} = @0", false);
        }
        return queryable;
    }

    public IQueryable<T> GetAllIQueryable<T>(bool tracking = false, CancellationToken cancellation = default) where T : class
    {
        var queryable = (tracking) switch
        {
            true => dbContext.Set<T>().IgnoreQueryFilters(),
            _ => dbContext.Set<T>().IgnoreQueryFilters().AsNoTracking()
        };
        return queryable;
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

public record FilterKeys
{
    public string Name { get; }
    private FilterKeys(string name)
    {
        this.Name = name;
    }

    public static FilterKeys SoftDeleted = new("SoftDeleted");
    public static FilterKeys Tenant = new("Tenant");
}