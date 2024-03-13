using Heatwave.Application.Interfaces;
using Heatwave.Domain;
using Heatwave.Infrastructure.DI;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Heatwave.Infrastructure.Persistence.Interceptors;

internal sealed class ISoftInterceptor : SaveChangesInterceptor, IScoped
{
    private readonly ICurrentUser currentUser;
    private readonly IDateTimeService dateTimeService;

    public ISoftInterceptor(ICurrentUser currentUser, IDateTimeService dateTimeService)
    {
        this.currentUser = currentUser;
        this.dateTimeService = dateTimeService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        OnSavingChanges(eventData);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        OnSavingChanges(eventData);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void OnSavingChanges(DbContextEventData eventData)
    {
        ArgumentNullException.ThrowIfNull(eventData.Context);
        eventData.Context.ChangeTracker.DetectChanges();
        foreach (var entityEntry in eventData.Context.ChangeTracker.Entries())
        {
            if (entityEntry is { State: EntityState.Deleted, Entity: ISoftDeleted softDeleteEntity })
            {
                softDeleteEntity.IsDeleted = true;
                entityEntry.State = EntityState.Modified;
                entityEntry.Property("IsDeleted").EntityEntry.State = EntityState.Modified;
                if (entityEntry.Entity is AuditableEntity auditable)
                {
                    auditable.DeletedBy = this.currentUser.UserId;
                    auditable.DeletedTime = this.dateTimeService.Current();
                    entityEntry.Property("DeletedBy").EntityEntry.State = EntityState.Modified;
                    entityEntry.Property("DeletedTime").EntityEntry.State = EntityState.Modified;
                }
            }
        }
    }

}
