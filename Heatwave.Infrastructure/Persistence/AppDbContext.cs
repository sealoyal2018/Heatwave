using Heatwave.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Heatwave.Application.Interfaces;

namespace Heatwave.Infrastructure.Persistence;

internal class AppDbContext : DbContext
{
    private readonly IDateTimeService dateTimeService;
    private readonly ICurrentUser currentUser;
    private readonly IMediator mediator;
    public AppDbContext(IDateTimeService dateTimeService, ICurrentUser currentUser, IMediator mediator)
    {
        this.dateTimeService = dateTimeService;
        this.currentUser = currentUser;
        this.mediator = mediator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        return base.SaveChanges();
    }
}