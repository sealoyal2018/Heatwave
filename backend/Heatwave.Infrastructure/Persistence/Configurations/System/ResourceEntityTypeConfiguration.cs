using Heatwave.Domain.System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Heatwave.Infrastructure.Persistence.Configurations.System;
internal class ResourceEntityTypeConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable("sys_resource", b => b.HasComment("资源"));

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
        builder.Property(v => v.Type).IsRequired().HasComment("资源类型（0：菜单，1：页面，2：按钮）");
        builder.Property(v => v.LinkType).HasComment("链接类型(0:内链，1：外链)");
        builder.Property(v => v.Value).HasComment("值");
        builder.Property(v => v.ParentId);
    }
}
