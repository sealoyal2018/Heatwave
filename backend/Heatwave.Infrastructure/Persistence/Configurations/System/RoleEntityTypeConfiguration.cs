using Heatwave.Domain.System;

using Microsoft.EntityFrameworkCore;

namespace Heatwave.Infrastructure.Persistence.Configurations.System;
internal class RoleEntityTypeConfiguration : IEntityTypeConfiguration<TenantRole>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TenantRole> builder)
    {

        builder.ToTable("sys_role", b => b.HasComment("角色表"));

        builder.Property(v => v.Id).IsRequired();
        builder.HasKey(v => v.Id);
        builder.Property(v => v.CreatedBy).IsRequired();
        builder.Property(v => v.CreatedTime).IsRequired();
        builder.Property(v => v.ModifiedBy).HasComment("修改者");
        builder.Property(v => v.ModifiedTime).HasComment("修改时间");
        builder.Property(v => v.DeletedBy).HasComment("删除者");
        builder.Property(v => v.DeletedTime).HasComment("删除时间");
        builder.Property(v => v.IsDeleted).HasComment("是否删除").IsRequired().HasDefaultValue(false);

        builder.Property(v => v.Name).HasMaxLength(50).IsRequired().HasComment("名称");
        builder.Property(v => v.Description).HasMaxLength(255).HasComment("描述");
    }
}
