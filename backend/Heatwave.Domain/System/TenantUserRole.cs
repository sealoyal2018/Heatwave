namespace Heatwave.Domain.System;

[Table("sys_tenant_user_role")]
public class TenantUserRole :  EntityBase, ITenant
{
    public long TenantId { get; set; }
    public long RoleId { get; set; }
    public long UserId { get; set; }

    public virtual TenantRole Role { get; set; }  
}
