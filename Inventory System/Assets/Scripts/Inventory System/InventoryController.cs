using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    [System.Serializable]
    public class InventoryController:IInventory, IInventoryCheck
    {
        public InventoryType Type => _inventoryDescriptor.Type;
        public Inventory  Inventory => _inventory;
        
        protected readonly  ItemsDataRepository _itemsDataRepository;
        protected readonly Inventory _inventory;
        protected readonly InventoryDescriptor _inventoryDescriptor;
        
        public InventoryController(ItemsDataRepository itemsDataRepository,InventoryDescriptor inventoryDescriptor)
        {
            _itemsDataRepository = itemsDataRepository;
            _inventory = new Inventory(_itemsDataRepository);
            _inventoryDescriptor = inventoryDescriptor;
        }

        public void Init()
        {
            var index = 0;
            foreach (var slotInitData in _inventoryDescriptor.SlotsToInit)
            {
                for (int i = 0; i < slotInitData.Count; i++)
                {
                    _inventory.AddSlot(new Slot(index +slotInitData.AditionalIndex, slotInitData.Tags,_inventoryDescriptor.Type));
                    _inventory.Slots[^1].SetAvailable(index < _inventoryDescriptor.BaseAvailableSlots);
                    index++;
                }
            }
        }

        public virtual bool HasItem(ItemName name)
        {
            return _inventory.HasItem(name);
        }

        public virtual bool HasFreeSlot()
        {
            return _inventory.HasFreeSlot();
        }

        public virtual ItemAddResult AddItem(ItemName itemName, int count)
        {
            return _inventory.AddItem(itemName, count);
        }

        public virtual ItemReduceResult ReduceItem(ItemName itemName, int count)
        {
            return _inventory.ReduceItem(itemName, count);
        }

        public virtual MoveItemResult MoveItemTo(ItemName itemName, int slotId)
        {
            var result = _inventory.MoveItemTo(itemName, slotId);
            if (result == MoveItemResult.SelectedSlotOccupied)
            {
                return SwapItemToOccupied(itemName, slotId);
            }

            return result;
        }

        public virtual SwapItemResult SwapItems(ItemName itemName, int slotId)
        {
            return _inventory.SwapItems(itemName, slotId);
        }

        private MoveItemResult SwapItemToOccupied(ItemName itemName, int slotId)
        {
            var result = SwapItems(itemName, slotId);
            
            if (result == SwapItemResult.Success)
            {
                return MoveItemResult.Success;
            }

            return MoveItemResult.SelectedSlotMissingTag;
        }

        public ItemAddResult CanAddItem(ItemName itemName, int count)
        {
            return _inventory.CanAddItem(itemName, count);
        }

        public ItemReduceResult CanReduceItem(ItemName itemName, int count)
        {
            return _inventory.CanReduceItem(itemName, count);
        }

        public MoveItemResult CanMoveItemTo(ItemName itemName, int slotId)
        {
            return _inventory.CanMoveItemTo(itemName, slotId);
        }

        public SwapItemResult CanSwapItems(ItemName itemName, int slotId)
        {
            return _inventory.CanSwapItems(itemName, slotId);
        }
    }

    public enum InventoryType
    {
        None,
        Player,
        Stock,
        Equipment,
    }

    [System.Serializable]
    public struct SlotInitData
    {
        public int AditionalIndex;
        public int Count;
        public SlotTag Tags;
    }
    
}
