using System.Linq.Expressions;
using System.Reflection;

using Heatwave.Domain;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Heatwave.Infrastructure.Persistence.Extensions;
internal static class ISoftDeletedExtensions
{
    public static EntityTypeBuilder AddSoftDeletedQueryFilter(this EntityTypeBuilder entityTypeBuilder)
    {
        var methodToCall = typeof(ISoftDeletedExtensions)
            .GetMethod(nameof(GetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)
            .MakeGenericMethod(entityTypeBuilder.Metadata.ContainingEntityType.ClrType);
        var filter = methodToCall.Invoke(null, new object[] { });
        return entityTypeBuilder.HasQueryFilter((LambdaExpression)filter);
    }

    /// <summary>
    /// 软删除
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    private static LambdaExpression GetSoftDeleteFilter<TEntity>() where TEntity : ISoftDeleted
    {
        Expression<Func<TEntity, bool>> filter = x => !x.IsDeleted;
        return filter;
    }
}
