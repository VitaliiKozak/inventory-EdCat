using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;
public class TestInventory : MonoBehaviour
{
    public ItemsDataRepository ItemsDataRepository;
    public List<InitSlotData> SlotsInitData;
    public Inventory Inventory;

    public int AvalibleCount = 5;
    private void Awake()
    {
        Init();
    }
    
    private void Init()
    {
        Inventory = new Inventory(ItemsDataRepository);
        for (int i = 0; i < SlotsInitData.Count; i++)
        {
            var slot = new Slot(i, SlotsInitData[i].Tags);
            slot.SetAvailable(i < AvalibleCount);
            Inventory.AddSlot(slot);
        }
    }
    [ContextMenu("Debug Items")]
    private void DebugTest()
    {
        Inventory.DebugItems();
    }
    [ContextMenu("Add Items")]
    private void DebugAddItem()
    {
        Debug.LogError(Inventory.AddItem(ItemName.LootCrate, 1).ToString());
    }
    [ContextMenu("Add Quest Items")]
    private void DebugAddQuestItem()
    {
        Debug.LogError(Inventory.AddItem(ItemName.Carmine, 1).ToString());
    }
    [ContextMenu("Reduce Items")]
    private void DebugReduceItem()
    {
        Debug.LogError(Inventory.ReduceItem(ItemName.LootCrate, 1).ToString());
    }
    [ContextMenu("Reduce Quest Items")]
    private void DebugReduceQuestItem()
    {
        Debug.LogError(Inventory.ReduceItem(ItemName.Carmine, 1).ToString());
    }
 
    [ContextMenu("Move Item to 1")]
    private void DebugMoveItemTo1()
    {
        Debug.LogError(Inventory.MoveItemTo(ItemName.LootCrate, 1).ToString());
    }
    [ContextMenu("Move Item to 5")]
    private void DebugMoveItemTo5()
    {
        Debug.LogError(Inventory.MoveItemTo(ItemName.LootCrate, 5).ToString());
    }
    [ContextMenu("Unlock Quest1 Slot")]
    private void DebugUnlockQuestSlot()
    {
        Inventory.Slots.Find(x =>x.Tags.HasFlag(SlotTag.Quest1)).SetAvailable(true);
    }
}


[System.Serializable]
public class InitSlotData
{
    public SlotTag Tags;
}
