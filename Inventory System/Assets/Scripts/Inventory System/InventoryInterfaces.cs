using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public interface IInventory
    {
        void Init();
        void Sort();
        
        ItemAddResult AddItem(ItemName itemName, int count);
        ItemReduceResult ReduceItem(ItemName itemName, int count);
        MoveItemResult MoveItemTo(ItemName itemName, int slotId);
        SwapItemResult SwapItems(ItemName itemName, int slotId);

        Slot GetSlot(int id);
        Slot GetSlot(ItemName itemName);
        Slot GetSlot(Predicate<Slot> match);
    }

    public interface IInventoryCheck
    {
        ItemAddResult CanAddItem(ItemName itemName, int count);
        ItemReduceResult CanReduceItem(ItemName itemName, int count);
        MoveItemResult CanMoveItemTo(ItemName itemName, int slotId);
        SwapItemResult CanSwapItems(ItemName itemName, int slotId);
        bool HasItem(ItemName name);
        bool HasFreeSlot();
        bool HasSlot(Func<Slot, bool> predicate);
    }
    public interface IInventoryInfo
    {
        InventoryType Type { get; }
        int GetItemCount();
    }
    
    public interface ICapacityProvider
    {
        int GetCapacity();
        SimpleEvent<int> OnCapacityChangeEvent { get; }
    }
    
    public interface IItemsDataRepository
    {
        ItemData GetItemData(ItemName name);
    }
}