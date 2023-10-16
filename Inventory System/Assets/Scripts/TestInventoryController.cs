using InventorySystem;
using UnityEngine;

public class TestInventoryController : MonoBehaviour
{
  public ItemsDataRepository ItemsDataRepository;
  public InventoryDescriptor Descriptor;
  public InventoryController InventoryController;
  public InventoryManagement InventoryManagement;
  private void Awake()
  {
    InventoryController = new InventoryController(ItemsDataRepository, Descriptor);
    InventoryController.Init();
    InventoryController.AddItem(ItemName.LootCrate, 1);
    InventoryController.AddItem(ItemName.Potato, 1);
    InventoryController.AddItem(ItemName.Carmine, 1);
    InventoryController.AddItem(ItemName.Backpack_1lvl, 1);
    InventoryController.Inventory.DebugItems();
    
    InventoryManagement.AddInventory(InventoryController);
  }
}
