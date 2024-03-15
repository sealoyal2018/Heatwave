using Heatwave.Domain.System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Heatwave.Infrastructure.Persistence.Configurations.System;
internal class RoleResourceEntityTypeConfiguration : IEntityTypeConfiguration<RoleResource>
{
    public void Configure(EntityTypeBuilder<RoleResource> builder)
    {
        builder.ToTable("sys_role_resource", t => t.HasComment("角色资源"));
        builder.Property(v => v.Id).IsRequired();
        builder.HasKey(v => v.Id);

        builder.Property(v=> v.ResourceId).IsRequired();
        builder.Property(v => v.RoleId).IsRequired();

        builder.HasOne(v => v.Resource);

        builder.HasIndex(v => v.RoleId);
    }
}
