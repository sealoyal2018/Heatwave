using System.Data;
using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;

namespace Heatwave.Domain;
/// <summary>
/// 数据库操作接口
/// </summary>
public interface IDbAccessor 
{
    #region 数据库相关

    /// <summary>
    /// 连接字符串
    /// </summary>
    string ConnectionString { get; }
    
    #endregion

    #region 事务
    /// <summary>
    /// 开始事务
    /// </summary>
    /// <param name="isolationLevel"></param>
    /// <returns></returns>
    Task BeginTransactionAsync(IsolationLevel isolationLevel);

    /// <summary>
    /// 提交事务
    /// </summary>
    void CommitTransaction();

    /// <summary>
    /// 回滚事务
    /// </summary>
    void RollbackTransaction();

    /// <summary>
    /// 释放事务
    /// </summary>
    void DisposeTransaction();
    #endregion


    #region 增加数据

    /// <summary>
    /// 添加单条记录
    /// </summary>
    /// <typeparam name="T">实体泛型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <param name="tracking">是否开启实体追踪</param>
    Task<int> InsertAsync<T>(T entity, bool tracking = false, CancellationToken cancellation = default) where T : class;

    /// <summary>
    /// 添加多条记录
    /// </summary>
    /// <typeparam name="T">实体泛型</typeparam>
    /// <param name="entities">实体对象集合</param>
    /// <param name="tracking">是否开启实体追踪</param>
    Task<int> InsertAsync<T>(List<T> entities, bool tracking = false, CancellationToken cancellation = default) where T : class;
    #endregion

    #region 删除数据

    /// <summary>
    /// 删除单条记录
    /// </summary>
    /// <typeparam name="T">实体泛型</typeparam>
    /// <param name="key">主键</param>
    Task<int> DeleteAsync<T>(long key, CancellationToken cancellation = default) where T : EntityBase;

    /// <summary>
    /// 删除多条记录
    /// </summary>
    /// <typeparam name="T">实体泛型</typeparam>
    /// <param name="keys">多条记录主键集合</param>
    Task<int> DeleteAsync<T>(List<long> keys, CancellationToken cancellation = default) where T : EntityBase;

    /// <summary>
    /// 删除所有记录
    /// </summary>
    /// <typeparam name="T">实体泛型</typeparam>
    Task<int> DeleteAllAsync<T>(CancellationToken cancellation = default) where T : class;

    /// <summary>
    /// 删除单条记录
    /// </summary>
    /// <typeparam name="T">实体泛型</typeparam>
    /// <param name="entity">实体对象</param>
    Task<int> DeleteAsync<T>(T entity, CancellationToken cancellation = default) where T : class;

    /// <summary>
    /// 删除多条记录
    /// </summary>
    /// <typeparam name="T">实体泛型</typeparam>
    /// <param name="entities">实体对象集合</param>
    Task<int> DeleteAsync<T>(List<T> entities, CancellationToken cancellation = default) where T : class;

    /// <summary>
    /// 按条件删除记录
    /// </summary>
    /// <typeparam name="T">实体泛型</typeparam>
    /// <param name="condition">筛选条件</param>
    Task<int> DeleteAsync<T>(Expression<Func<T, bool>> condition, CancellationToken cancellation = default) where T : class;

    #endregion

    #region 更新数据

    /// <summary>
    /// 更新单条记录
    /// </summary>
    /// <typeparam name="T">实体泛型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <param name="tracking">是否开启实体追踪</param>
    Task<int> UpdateAsync<T>(T entity, bool tracking = false, CancellationToken cancellation = default) where T : class;

    /// <summary>
    /// 更新多条记录
    /// </summary>
    /// <typeparam name="T">实体泛型</typeparam>
    /// <param name="entities">实体对象集合</param>
    /// <param name="tracking">是否开启实体追踪</param>
    Task<int> UpdateAsync<T>(List<T> entities, bool tracking = false, CancellationToken cancellation = default) where T : class;

    /// <summary>
    /// 更新单条记录的某些属性
    /// </summary>
    /// <typeparam name="T">实体泛型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <param name="properties">属性</param>
    /// <param name="tracking">是否开启实体追踪</param>
    Task<int> UpdateAsync<T>(T entity, List<string> properties, bool tracking = false, CancellationToken cancellation = default) where T : class;

    /// <summary>
    /// 更新多条记录的某些属性
    /// </summary>
    /// <typeparam name="T">实体泛型</typeparam>
    /// <param name="entities">实体对象集合</param>
    /// <param name="properties">属性</param>
    /// <param name="tracking">是否开启实体追踪</param>
    Task<int> UpdateAsync<T>(List<T> entities, List<string> properties, bool tracking = false, CancellationToken cancellation = default) where T : class;

    /// <summary>
    /// 按照条件更新记录
    /// </summary>
    /// <typeparam name="T">实体泛型</typeparam>
    /// <param name="whereExpre">筛选条件</param>
    /// <param name="action">更新操作</param>
    /// <param name="tracking">是否开启实体追踪</param>
    Task<int> UpdateAsync<T>(Expression<Func<T, bool>> whereExpre, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> action, bool tracking = false, CancellationToken cancellation = default) where T : class;
    #endregion

    #region 查询数据

    /// <summary>
    /// 获取IQueryable
    /// 注:默认取消实体追踪
    /// </summary>
    /// <typeparam name="T">实体泛型</typeparam>
    /// <param name="tracking">是否开启实体追踪</param>
    /// <returns></returns>
    IQueryable<T> GetIQueryable<T>(bool tracking = false, CancellationToken cancellation = default) where T : class;

    /// <summary>
    /// 获取IQueryable
    /// 注:默认取消实体追踪
    /// </summary>
    /// <typeparam name="T">实体泛型</typeparam>
    /// <param name="tracking">是否开启实体追踪</param>
    /// <returns></returns>
    IQueryable<T> GetAllIQueryable<T>(bool tracking = false, CancellationToken cancellation = default) where T : class;

    #endregion
}