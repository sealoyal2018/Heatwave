using Heatwave.Domain.System;

using Microsoft.EntityFrameworkCore;

namespace Heatwave.Infrastructure.Persistence.Configurations.System;
internal class UserTokenEntityTypeConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable("sys_user_token", b => b.HasComment("用户登录"));

        builder.Property(v => v.Id).IsRequired();
        builder.HasKey(v => v.Id);
        builder.Property(v => v.CreatedBy).IsRequired();
        builder.Property(v => v.CreatedTime).IsRequired();
        builder.Property(v => v.ModifiedBy).HasComment("修改者");
        builder.Property(v => v.ModifiedTime).HasComment("修改时间");
        builder.Property(v => v.DeletedBy).HasComment("删除者");
        builder.Property(v => v.DeletedTime).HasComment("删除时间");
        builder.Property(v => v.IsDeleted).HasComment("是否删除").IsRequired().HasDefaultValue(false);

        builder.Property(v => v.UserId).IsRequired();
        builder.Property(v => v.Token).IsRequired().HasMaxLength(255).HasComment("token");
        builder.Property(v => v.TokenHash).IsRequired().HasMaxLength(255).HasComment("token哈希,用于查询");
        builder.Property(v => v.RefreshToken).HasMaxLength(255).HasComment("token刷新");
        builder.Property(v => v.ExpirationDate).IsRequired().HasComment("token过期时间");
        builder.Property(v => v.IpAddress).HasComment("获取token ip地址");
        builder.Property(v => v.RefreshTokenIsAvailable).IsRequired().HasComment("refresh token是否有效");

        builder.HasOne(v => v.User);
    }
}
