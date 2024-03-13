namespace Heatwave.Domain.System;
public class TenantUser : EntityBase, ITenant
{
    public long UserId { get; set; }
    public long TenantId { get; set; }
}
