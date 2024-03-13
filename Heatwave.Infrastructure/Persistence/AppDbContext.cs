using Heatwave.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Heatwave.Application.Interfaces;
using System.Data;
using Heatwave.Domain.System;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using Heatwave.Infrastructure.Persistence.Interceptors;
using Heatwave.Infrastructure.Persistence.Extensions;
using Z.EntityFramework.Plus;
using Z.EntityFramework.Extensions.EFCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Extensions.Options;

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
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
            var builder = modelBuilder.Entity(type);
            if (type.IsAssignableFrom(typeof(ISoftDeleted)))
                builder = ISoftDeletedExtensions.AddSoftDeletedQueryFilter(builder);
        }
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}