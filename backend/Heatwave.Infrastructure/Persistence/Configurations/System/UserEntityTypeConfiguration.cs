using Heatwave.Domain.System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Heatwave.Infrastructure.Persistence.Configurations.System;
internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("sys_user", b => b.HasComment("用户表"));

        builder.Property(v => v.Id).IsRequired();
        builder.HasKey(v => v.Id);
        builder.Property(v => v.CreatedBy).IsRequired();
        builder.Property(v => v.CreatedTime).IsRequired();
        builder.Property(v => v.ModifiedBy).HasComment("修改者");
        builder.Property(v => v.ModifiedTime).HasComment("修改时间");
        builder.Property(v => v.DeletedBy).HasComment("删除者");
        builder.Property(v => v.DeletedTime).HasComment("删除时间");
        builder.Property(v => v.IsDeleted).HasComment("是否删除").IsRequired().HasDefaultValue(false);

        builder.Property(v => v.NickName).HasMaxLength(50).HasComment("名称");
        builder.Property(v => v.Email).HasMaxLength(50).HasComment("邮箱");
        builder.Property(v => v.Password).HasMaxLength(50).IsRequired().HasComment("密码");
        builder.Property(v => v.PhoneNumber).HasMaxLength(15).HasComment("电话号码");
        builder.Property(v => v.UserType).IsRequired().HasComment("用户类型");

        builder.HasIndex(v => v.PhoneNumber);
        builder.HasIndex(v => v.Email);
    }
}
