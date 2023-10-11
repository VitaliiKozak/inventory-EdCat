using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using UnityEngine;

public class TestInventoryInteraction : MonoBehaviour
{
    public TestInventoryController FirstInventory;
    public TestInventoryController SecondInventory;

    [ContextMenu("TestMove")]
    private void TestMove()
    {
        FirstInventory.InventoryController.Inventory.DebugItems();
        SecondInventory.InventoryController.Inventory.DebugItems();
        
        var slot = FirstInventory.InventoryController.Inventory.Slots.Find(x => x.IsFree == false);
        if (SecondInventory.InventoryController.AddItem(slot.Item.Name, slot.Count) == ItemAddResult.Success)
        {
            slot.Reduce(slot.Count);
        }

        FirstInventory.InventoryController.Inventory.DebugItems();
        SecondInventory.InventoryController.Inventory.DebugItems();
    }
}
