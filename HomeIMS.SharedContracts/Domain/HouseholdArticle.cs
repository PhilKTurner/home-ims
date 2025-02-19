namespace HomeIMS.SharedContracts.Domain;

public class HouseholdArticle : Article, IHouseholdArticle
{
    public string? EAN { get; set; }
    public uint? TargetAmount { get; set; }
}