using System.ComponentModel.DataAnnotations.Schema;

namespace Heatwave.Domain.System;

[Table("sys_tenant_role_resource")]
public class TenantRoleResource : EntityBase, ITenant
{
    public long TenantId { get; set; }
    public long RoleId { get; set; }

    public long ResourceId { get; set; }

    public virtual Resource Resource { get; set; }
}
