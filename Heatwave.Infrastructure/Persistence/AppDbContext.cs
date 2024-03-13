using Heatwave.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Heatwave.Application.Interfaces;
using System.Data;
using Heatwave.Domain.System;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heatwave.Infrastructure.Persistence;

internal class AppDbContext : DbContext
{
    private readonly IDateTimeService dateTimeService;
    private readonly ICurrentUser currentUser;
    private readonly IMediator mediator;

    public AppDbContext(DbContextOptions<AppDbContext> options, IDateTimeService dateTimeService, ICurrentUser currentUser, IMediator mediator)
        :base(options)
    {
        this.dateTimeService = dateTimeService;
        this.currentUser = currentUser;
        this.mediator = mediator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assemblies = typeof(IEntity).Assembly;
        var typesToRegister = assemblies
            .GetTypes()
            .Where(p => p.IsAssignableTo(typeof(IEntity)) && p.GetCustomAttribute<TableAttribute>() != null)
            .ToList();

        foreach (Type type in typesToRegister)
        {
            modelBuilder.Entity(type);
        }

        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
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