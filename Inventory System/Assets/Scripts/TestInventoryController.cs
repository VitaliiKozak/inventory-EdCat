using System;
using InventorySystem;
using UI;
using UnityEngine;

public class TestInventoryController : MonoBehaviour
{
  public ItemsDataRepository ItemsDataRepository;
  public InventoryManagement InventoryManagement;
  
  //Stock
  public Inventory StockInventory;
  public InventoryDescriptor StockDescriptor;
  public StockController StockView; 
  
  //equipment
  public Inventory EquipmentInventory;
  public InventoryDescriptor EquipmentDescriptor; 
  public StockController EquipmentView;
  
  //player
  public Inventory PlayerInventory;
  public InventoryDescriptor PlayerDescriptor;
  public StockController PlayerView;
  
  private void Start()
  {
    InitStockInventory();
    InitEquipmentInventory();
    InitPlayerInventory();
  }

  private void InitPlayerInventory()
  {
    var provider = new EquipmentCapacityProvider(EquipmentInventory.Slots.Find(x => x.Tags.HasFlag(SlotTag.Backpack)));
    PlayerInventory = new Inventory(ItemsDataRepository, PlayerDescriptor, provider);
    PlayerInventory.Init();
    PlayerInventory.AddItem(ItemName.LootCrate, 1);
    PlayerInventory.AddItem(ItemName.Potato, 1);
    PlayerInventory.AddItem(ItemName.Carmine, 1);
    PlayerInventory.AddItem(ItemName.Backpack_2lvl, 1);
    PlayerInventory.AddItem(ItemName.Weapon_TypeA, 2);
    PlayerInventory.AddItem(ItemName.Weapon_TypeB, 5);
    PlayerInventory.DebugItems();
    
    InventoryManagement.AddInventory(PlayerInventory);
    PlayerView?.TestInit(PlayerInventory);
  }
  private void InitStockInventory()
  {
    StockInventory = new Inventory(ItemsDataRepository, StockDescriptor, new InfinityCapacityProvider());
    StockInventory.Init();
    StockInventory.AddItem(ItemName.LootCrate, 1);
    StockInventory.AddItem(ItemName.Potato, 1);
    StockInventory.AddItem(ItemName.Carmine, 1);
    StockInventory.DebugItems();
    
    InventoryManagement.AddInventory(StockInventory);
    StockView?.TestInit(StockInventory);
  }
  
  private void InitEquipmentInventory()
  {
    EquipmentInventory = new Inventory(ItemsDataRepository, EquipmentDescriptor, new InfinityCapacityProvider());
    EquipmentInventory.Init();
    EquipmentInventory.AddItem(ItemName.LootCrate, 1);
    EquipmentInventory.AddItem(ItemName.Potato, 1);
    EquipmentInventory.AddItem(ItemName.Carmine, 1);
    EquipmentInventory.AddItem(ItemName.Backpack_1lvl, 1);
    EquipmentInventory.DebugItems();
    
    InventoryManagement.AddInventory(EquipmentInventory);
    EquipmentView?.TestInit(EquipmentInventory);
  }

  [ContextMenu("Test Sort")]
  private void TestSort()
  {
    PlayerInventory.Sort();
  }
}
