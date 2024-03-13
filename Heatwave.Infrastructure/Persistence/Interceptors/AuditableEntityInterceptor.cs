using Heatwave.Domain;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Heatwave.Application.Interfaces;

namespace Heatwave.Infrastructure.Persistence.Interceptors;

internal class AuditableEntityInterceptor : SaveChangesInterceptor, IScoped
{
    private readonly ICurrentUser currentUser;
    private readonly IDateTimeService dateTimeService;

    public AuditableEntityInterceptor(ICurrentUser currentUser, IDateTimeService dateTimeService)
    {
        this.currentUser = currentUser;
        this.dateTimeService = dateTimeService;
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = currentUser.UserId;
                entry.Entity.CreatedTime = dateTimeService.Current();
            }

            if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.ModifiedBy = currentUser.UserId;
                entry.Entity.ModifiedTime = dateTimeService.Current();
            }

            if (entry.State == EntityState.Deleted)
            {
                entry.Entity.DeletedBy = currentUser.UserId;
                entry.Entity.DeletedTime = dateTimeService.Current();
            }
        }
    }
}


internal static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}