using Heatwave.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Heatwave.Application.Interfaces;
using System.Data;
using Heatwave.Domain.System;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using Heatwave.Infrastructure.Persistence.Extensions;
using Z.EntityFramework.Plus;
using System.Linq.Expressions;

namespace Heatwave.Infrastructure.Persistence;

internal class AppDbContext : DbContext
{
    private readonly IDateTimeService dateTimeService;
    private readonly ICurrentUser currentUser;
    private readonly IMediator mediator;

    public AppDbContext(DbContextOptions<AppDbContext> options, IDateTimeService dateTimeService, ICurrentUser currentUser, IMediator mediator)
        : base(options)
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

            Expression<Func<ITenant, bool>> expr = e => currentUser.TenantIds.Contains(e.TenantId);
            //if (typeof(ITenant).IsAssignableFrom(type))
            //{
            //    modelBuilder.Entity(type).Property<long>(nameof(ITenant.TenantId));
            //    var parameter = Expression.Parameter(type, "e");
            //    var body = Expression.Equal(Expression.Call(typeof(EF), nameof(EF.Property), new[] { typeof(bool) }, parameter, Expression.Constant(nameof(ITenant.TenantId))), Expression.Constant(false));
            //    modelBuilder.Entity(type).HasQueryFilter(Expression.Lambda(body, parameter));
            //}
        }
        this.Filter<User>(FilterKeys.SoftDeleted, t => t.Where(v => !v.IsDeleted));
        this.Filter<ITenant>(FilterKeys.Tenant, t => t.Where(v => v.TenantId == 3));

        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

}