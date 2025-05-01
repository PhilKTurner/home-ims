using HomeIMS.SharedContracts.Commands;

namespace HomeIMS.SharedContracts.Domain.Inventories.Commands;

public class UpdateInventory : HimsCommand, IInventory
{
    public UpdateInventory()
    {
    }

    public UpdateInventory(Inventory inventoryToUpdate)
    {
        Id = inventoryToUpdate.Id;
        ArticleGroupId = inventoryToUpdate.ArticleGroupId;
        Name = inventoryToUpdate.Name;
        Description = inventoryToUpdate.Description;
    }

    public Guid Id { get; set; }

    public Guid? ArticleGroupId { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
}
