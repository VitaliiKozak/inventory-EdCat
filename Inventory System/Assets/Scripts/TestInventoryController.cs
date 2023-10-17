using InventorySystem;
using UnityEngine;

public class TestInventoryController : MonoBehaviour
{
  public ItemsDataRepository ItemsDataRepository;
  public InventoryDescriptor Descriptor;
  public Inventory InventoryController;
  public InventoryManagement InventoryManagement;
  private void Start()
  {
    InventoryController = new Inventory(ItemsDataRepository, Descriptor, new InfinityCapacityProvider());
    InventoryController.Init();
    InventoryController.AddItem(ItemName.LootCrate, 1);
    InventoryController.AddItem(ItemName.Potato, 1);
    InventoryController.AddItem(ItemName.Carmine, 1);
    InventoryController.AddItem(ItemName.Backpack_1lvl, 1);
    InventoryController.DebugItems();
    
    InventoryManagement.AddInventory(InventoryController);
  }
}
