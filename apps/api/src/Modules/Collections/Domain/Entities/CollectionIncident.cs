using Api.Shared.Domain.Exceptions;

namespace Api.Modules.Collections.Domain.Entities;

public sealed class CollectionIncident
{
    private CollectionIncident()
    {
    }

    public CollectionIncident(Guid id, Guid collectionId, string description, string responsibleUser, DateTimeOffset registeredAt)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new BusinessRuleException("Incident description is required.", "incident_description_required");
        }

        if (string.IsNullOrWhiteSpace(responsibleUser))
        {
            throw new BusinessRuleException("Incident responsible user is required.", "incident_responsible_user_required");
        }

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        CollectionId = collectionId;
        Description = description.Trim();
        ResponsibleUser = responsibleUser.Trim();
        RegisteredAt = registeredAt;
    }

    public Guid Id { get; private set; }
    public Guid CollectionId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string ResponsibleUser { get; private set; } = string.Empty;
    public DateTimeOffset RegisteredAt { get; private set; }
}
