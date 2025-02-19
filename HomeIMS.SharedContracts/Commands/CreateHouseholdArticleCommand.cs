using HomeIMS.SharedContracts.Domain;

namespace HomeIMS.SharedContracts.Commands
{
    public class CreateHouseholdArticleCommand : Command, IHouseholdArticle
    {
        public CreateHouseholdArticleCommand()
        {
            Type = CommandType.CreateHouseholdArticle;
        }

        public Guid Id { get; set; }
        public Guid? GroupId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? EAN { get; set; }
        public uint? TargetAmount { get; set; }
    }
}