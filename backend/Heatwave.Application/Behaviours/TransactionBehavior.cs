using System.Data;
using System.Reflection;

using Heatwave.Domain;

namespace Heatwave.Application.Behaviours;

internal class TransactionBehavior<TRequest, TReposonse> : IPipelineBehavior<TRequest, TReposonse>
    where TRequest : notnull
{
    private readonly IDbAccessor dbAccessor;
    private bool isOpened;

    public TransactionBehavior(IDbAccessor dbAccessor)
    {
        this.isOpened = false;
        this.dbAccessor = dbAccessor;
    }

    public async Task<TReposonse> Handle(TRequest request, RequestHandlerDelegate<TReposonse> next, CancellationToken cancellationToken)
    {
        var requestType = request.GetType();
        if (typeof(ICommand).IsAssignableFrom(requestType) || typeof(ICommand<>).IsAssignableFrom(requestType))
        {
            var transaction = next.Method.GetCustomAttribute<TransactionAttribute>();
            if (transaction is null)
                return await next();
            
            if (isOpened)
                return await next();

            try
            {
                await dbAccessor.BeginTransactionAsync(transaction.IsolationLevel);
                isOpened = true;
                var w = await next();
                dbAccessor.CommitTransaction();
                return w;
            }
            catch (Exception ex)
            {
                dbAccessor.RollbackTransaction();
                throw;
            }
            finally
            {
                dbAccessor.DisposeTransaction();
            }
        }
        return await next();
    }
}

public class TransactionAttribute: Attribute
{
    public IsolationLevel IsolationLevel { get; set; }
}