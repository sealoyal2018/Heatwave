using System.ComponentModel.DataAnnotations.Schema;

namespace Heatwave.Domain.System;

[Table("sys_tenant_resource")]
public class TenantResource : EntityBase, ITenant
{
    public long TenantId { get; set; }
    public long ResourceId { get; set; }

    public virtual Resource Resource { get; set; }
}
