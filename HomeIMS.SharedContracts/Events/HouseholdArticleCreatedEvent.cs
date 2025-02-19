using HomeIMS.SharedContracts.Domain;

namespace HomeIMS.SharedContracts.Events;

public class HouseholdArticleCreatedEvent : Event, IHouseholdArticle
{
    public HouseholdArticleCreatedEvent(DateTimeOffset timestamp) : base(timestamp)
    {
    }

    public Guid Id { get; set; }
    public Guid? GroupId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? EAN { get; set; }
    public uint? TargetAmount { get; set; }
}