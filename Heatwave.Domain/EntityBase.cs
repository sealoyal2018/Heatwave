using System.Text.Json.Serialization;

using MediatR;

namespace Heatwave.Domain;
public abstract class EntityBase : IEntity
{
    public long Id { get; set; }

    private List<INotification> _domainEvents;

    [JsonIgnore]
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents = _domainEvents ?? new List<INotification>();
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        _domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }
}
