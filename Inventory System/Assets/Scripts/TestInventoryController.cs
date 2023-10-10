using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using UnityEngine;

public class TestInventoryController : MonoBehaviour
{
  public ItemsDataRepository ItemsDataRepository;
  public InventoryDescriptor Descriptor;
  public InventoryController InventoryController;

  private void Awake()
  {
    InventoryController = new InventoryController(ItemsDataRepository, Descriptor);
    InventoryController.Init();
    InventoryController.Inventory.AddItem(ItemName.LootCrate, 1);
    InventoryController.Inventory.DebugItems();
  }
}
