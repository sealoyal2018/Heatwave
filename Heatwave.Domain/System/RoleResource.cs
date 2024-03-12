using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;

namespace Heatwave.Domain.System;

public class RoleResource : EntityBase
{
    public long RoleId { get; set; }

    public long ResourceId { get; set; }

    public virtual Resource Resource { get; set; }
}
