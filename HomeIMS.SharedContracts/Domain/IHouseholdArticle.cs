namespace HomeIMS.SharedContracts.Domain;

public interface IHouseholdArticle : IArticle
{
    string? EAN { get; set; }
    uint? TargetAmount { get; set; }
}
