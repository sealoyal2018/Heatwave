namespace Heatwave.Domain.System;
public class TenantUserRole :  EntityBase, ITenant
{
    public long TenantId { get; set; }
    public long RoleId { get; set; }
    public long UserId { get; set; }
    public virtual TenantRole Role { get; set; }
}
