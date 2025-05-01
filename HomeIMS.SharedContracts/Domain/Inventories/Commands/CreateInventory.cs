using HomeIMS.SharedContracts.Commands;

namespace HomeIMS.SharedContracts.Domain.Inventories.Commands;

public class CreateInventory : HimsCommand, IInventory
{
    public Guid? ArticleGroupId { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }

    public CreateInventory()
    {
    }

    public CreateInventory(Inventory inventoryToCreate)
    {
        ArticleGroupId = inventoryToCreate.ArticleGroupId;
        Name = inventoryToCreate.Name;
        Description = inventoryToCreate.Description;
    }
}
