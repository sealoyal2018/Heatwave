using Heatwave.Domain.System;

using Microsoft.EntityFrameworkCore;

namespace Heatwave.Infrastructure.Persistence.Configurations.System;
internal class UserRoleEntityTypeConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("sys_user_role", t => t.HasComment("用户角色"));

        builder.Property(v => v.Id).IsRequired();
        builder.HasKey(v => v.Id);

        builder.Property(v => v.RoleId).IsRequired();
        builder.Property(v => v.UserId).IsRequired();

        builder.OwnsOne(v => v.Role);

        builder.HasIndex(v => v.UserId);
    }
}
