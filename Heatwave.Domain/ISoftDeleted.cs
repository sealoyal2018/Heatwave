namespace Heatwave.Domain;
public interface ISoftDeleted
{
    bool IsDeleted { get; set; }
}
