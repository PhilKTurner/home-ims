public interface IInventory
{
    Guid Id { get; }
    Guid? ArticleGroupId { get; set; }

    string? Name { get; set; }
    string? Description { get; set; }
}