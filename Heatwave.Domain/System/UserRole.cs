namespace Heatwave.Domain.System;
public class UserRole : EntityBase
{
    public long RoleId { get; set; }
    public long UserId { get; set; }
    public virtual Role Role { get; set; }
}
