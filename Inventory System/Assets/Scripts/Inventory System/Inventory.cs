using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    [System.Serializable]
    public class Inventory : IInventory, IInventoryCheck, IInventoryInfo
    {
        public InventoryType Type => _inventoryDescriptor.Type;

        public List<Slot> Slots { get; private set; }
        
        protected readonly IItemsDataRepository _repository;
        protected readonly InventoryDescriptor _inventoryDescriptor;
        protected readonly ICapacityProvider _capacityProvider;
        public Inventory(IItemsDataRepository repository,InventoryDescriptor inventoryDescriptor,ICapacityProvider capacityProvider)
        {
            _repository = repository;
            _inventoryDescriptor = inventoryDescriptor;
            _capacityProvider = capacityProvider;
            Slots = new List<Slot>();
        }
        public void AddSlot(Slot slot)
        {
            Slots.Add(slot);
        }
        public void Init()
        {
            var index = 0;
            var availableSlots = _capacityProvider.GetCapacity();
            foreach (var slotInitData in _inventoryDescriptor.SlotsToInit)
            {
                for (int i = 0; i < slotInitData.Count; i++)
                {
                    AddSlot(new Slot(index +slotInitData.AditionalIndex, slotInitData.Tags,_inventoryDescriptor.Type));
                    Slots[^1].SetAvailable(index < availableSlots);
                    index++;
                }
            }
            _capacityProvider.OnCapacityChangeEvent.AddListener(ChangeCapacityCallback);
            ChangeCapacityCallback(_capacityProvider.GetCapacity());
        }

        public void Sort()
        {
            var makeChanges = false;
            do
            {
                makeChanges = false;
                for (int i = Slots.Count-1; i >= 0; i--)
                {
                    if(Slots[i].IsFree == true) continue;

                    var slotFree = GetSlot(x => x.IsFree == true && (x.Tags & Slots[i].Item.SlotsData) != SlotTag.Nothing);
                    if(slotFree == null || slotFree.Id > Slots[i].Id) continue;
               
                    slotFree.SetItem(Slots[i].Item, Slots[i].Count);
                    Slots[i].Reduce(Slots[i].Count);
                    makeChanges = true;
                }
                
            } while (makeChanges);
        }

        public bool HasItem(ItemName name)
        {
            return Slots.Any(x => x.IsFree == false && x.Item.Name == name);
        }

        public bool HasFreeSlot()
        {
            return Slots.Any(x => x.IsFree == true && x.IsAvailable == true);
        }

        public ItemAddResult AddItem(ItemName itemName, int count)
        {
            if (HasItem(itemName) == true)
            {
                GetSlot(itemName).Add(count);
                return ItemAddResult.Success;
            }

            if (HasFreeSlot() == false) return ItemAddResult.AbsenceOfEmptySlots;
            var data = _repository.GetItemData(itemName);
            var slot = Slots.Find(x => x.IsFree == true && x.IsAvailable == true && (x.Tags & data.SlotsData) != SlotTag.Nothing);

            if (slot == null) return ItemAddResult.AbsenceOfSlotsWithSuitableTag;
            
            slot.SetItem(data,count);
            return ItemAddResult.Success;
        }

        public ItemReduceResult ReduceItem(ItemName itemName, int count)
        {
            if (HasItem(itemName) == false) return ItemReduceResult.AbsenceOfItem;

            var slot = GetSlot(itemName);
            
            if(slot.CanGet(count) == false) return ItemReduceResult.InsufficientQuantity;

            slot.Reduce(count);
            
            return ItemReduceResult.Success;
        }
        
        public MoveItemResult MoveItemTo(ItemName itemName, int slotId)
        {
            if (HasItem(itemName) == false) return MoveItemResult.NoSuchItem;

            var preferSlot = GetSlot(slotId);
            if(preferSlot == null) return MoveItemResult.NoSelectedSlot;
            
            if(preferSlot.IsAvailable == false)return MoveItemResult.SelectedSlotNotAvailable;
            if(preferSlot.IsFree == false) return SwapItemToOccupied(itemName, slotId);

            var currentSlot = GetSlot(itemName);
            if(preferSlot.Id == currentSlot.Id ) return MoveItemResult.CurrentSlotMatchesSelected;
            if((preferSlot.Tags & currentSlot.Item.SlotsData) == SlotTag.Nothing)  return MoveItemResult.SelectedSlotMissingTag;
            
            var count = currentSlot.Count;
            var data = currentSlot.Item;
            currentSlot.Reduce(count);
            
            preferSlot.SetItem(data, count);
            
            return MoveItemResult.Success;
        }
        
        public SwapItemResult SwapItems(ItemName itemName, int slotId)
        {
            if (HasItem(itemName) == false) return SwapItemResult.NoSuchItem;

            var preferSlot = GetSlot(slotId);
            if(preferSlot == null) return SwapItemResult.NoSelectedSlot;
            
            if(preferSlot.IsAvailable == false) return SwapItemResult.SelectedSlotNotAvailable;
            if(preferSlot.IsFree == true) return SwapItemResult.SelectedSlotEmpty;
            
            var currentSlot = GetSlot(itemName);
            if(preferSlot.Id == currentSlot.Id ) return SwapItemResult.CurrentSlotMatchesSelected;

            if((preferSlot.Tags & currentSlot.Item.SlotsData) == SlotTag.Nothing) return SwapItemResult.SelectedSlotTagMismatch;
            if((currentSlot.Tags & preferSlot.Item.SlotsData) == SlotTag.Nothing) return SwapItemResult.CurrentSlotTagMismatch;

            var count = currentSlot.Count;
            var data = currentSlot.Item;
            
            currentSlot.SetItem(preferSlot.Item, preferSlot.Count);
            preferSlot.SetItem(data, count);
            
            return SwapItemResult.Success;
        }

        public Slot GetSlot(int id)
        {
            return Slots.Find(x => x.Id == id);
        }

        public Slot GetSlot(ItemName itemName)
        {
            return Slots.Find(x => x.IsFree == false && x.Item.Name == itemName);
        }

        public Slot GetSlot(Predicate<Slot> match)
        {
            return Slots.Find(match);
        }

        public void DebugItems()
        {
            var data = "Inventory:\n";
            for (int i = 0; i < Slots.Count; i++)
            {
                data+=Slots[i].ToString() + "\n";
            }
            Debug.LogError(data);
        }

        public ItemAddResult CanAddItem(ItemName itemName, int count)
        {
            if (HasItem(itemName) == true) return ItemAddResult.Success;
            if (HasFreeSlot() == false) return ItemAddResult.AbsenceOfEmptySlots;
            var data = _repository.GetItemData(itemName);
            var slot = Slots.Find(x => x.IsFree == true && x.IsAvailable == true && (x.Tags & data.SlotsData) != SlotTag.Nothing);
            if (slot == null) return ItemAddResult.AbsenceOfSlotsWithSuitableTag;
            return ItemAddResult.Success;
        }

        public ItemReduceResult CanReduceItem(ItemName itemName, int count)
        {
            if (HasItem(itemName) == false) return ItemReduceResult.AbsenceOfItem;
            var slot = GetSlot(itemName);
            if(slot.CanGet(count) == false) return ItemReduceResult.InsufficientQuantity;
            return ItemReduceResult.Success;
        }

        public MoveItemResult CanMoveItemTo(ItemName itemName, int slotId)
        {
            if (HasItem(itemName) == false) return MoveItemResult.NoSuchItem;
            
            var preferSlot = GetSlot(slotId);
            if(preferSlot == null) return MoveItemResult.NoSelectedSlot;
            
            if(preferSlot.IsAvailable == false)return MoveItemResult.SelectedSlotNotAvailable;
            if(preferSlot.IsFree == false)return MoveItemResult.SelectedSlotOccupied;
            
            var currentSlot = GetSlot(itemName);
            if(preferSlot.Id == currentSlot.Id ) return MoveItemResult.CurrentSlotMatchesSelected;

            if((preferSlot.Tags & currentSlot.Item.SlotsData) == SlotTag.Nothing)  return MoveItemResult.SelectedSlotMissingTag;
            
            return MoveItemResult.Success;
        }

        public SwapItemResult CanSwapItems(ItemName itemName, int slotId)
        {
            if (HasItem(itemName) == false) return SwapItemResult.NoSuchItem;

            var preferSlot = GetSlot(slotId);
            if(preferSlot == null) return SwapItemResult.NoSelectedSlot;
            
            if(preferSlot.IsAvailable == false) return SwapItemResult.SelectedSlotNotAvailable;
            if(preferSlot.IsFree == true) return SwapItemResult.SelectedSlotEmpty;
            
            var currentSlot = GetSlot(itemName);
            if(preferSlot.Id == currentSlot.Id ) return SwapItemResult.CurrentSlotMatchesSelected;
            
            if((preferSlot.Tags & currentSlot.Item.SlotsData) == SlotTag.Nothing) return SwapItemResult.SelectedSlotTagMismatch;
            if((currentSlot.Tags & preferSlot.Item.SlotsData) == SlotTag.Nothing)  return SwapItemResult.CurrentSlotTagMismatch;

            return SwapItemResult.Success;
        }

        public bool HasSlot(Func<Slot, bool> predicate)
        {
            return Slots.Any(predicate);
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
        
        private void ChangeCapacityCallback(int count)
        {
            for (int i = 0; i < Slots.Count; i++)
            {
                Slots[i].SetAvailable(i < count);
            }
        }
        
        public int GetItemCount()
        {
            return Slots.FindAll(x => x.IsFree == false && x.IsAvailable == true).Count;
        }
    }

    public enum ItemAddResult
    {
        None,                         // No issues
        AbsenceOfEmptySlots,          // Indicates the absence of empty slots
        AbsenceOfSlotsWithSuitableTag,// Indicates the absence of slots with a suitable tag
        Success,                      // Indicates success
    }

    public enum ItemReduceResult
    {
        None,                   // No issues
        Success,                // Indicates success
        AbsenceOfItem,          // Indicates the absence of the item
        InsufficientQuantity,   // Indicates insufficient quantity of the item
    }

    public enum MoveItemResult
    {
        None,                       // No issues
        Success,                    // Indicates success
        NoSuchItem,                 // Indicates that there is no such item
        NoSelectedSlot,             // Indicates that no slot is selected
        SelectedSlotNotAvailable,   // Indicates that the selected slot is not available
        SelectedSlotOccupied,       // Indicates that the selected slot is already occupied
        CurrentSlotMatchesSelected, // Indicates that the current slot matches the selected slot
        SelectedSlotMissingTag,     // Indicates that the selected slot doesn't have the required tag
    }

    public enum SwapItemResult
    {
        None,                       // No issues
        Success,                    // Indicates success
        NoSuchItem,                 // Indicates that there is no such item
        NoSelectedSlot,             // Indicates that no slot is selected
        SelectedSlotNotAvailable,   // Indicates that the selected slot is not available
        SelectedSlotEmpty,          // Indicates that the selected slot is empty
        CurrentSlotMatchesSelected, // Indicates that the current slot matches the selected slot
        SelectedSlotTagMismatch,    // Indicates that the selected slot doesn't match required tags
        CurrentSlotTagMismatch,      // Indicates that the current slot doesn't match required tags
    }

    public enum EquipItemResult
    {
        None,                       // No issues
        Success,                    // Indicates success
        CurrentSlotEmpty = 2,                // Indicates that the current slot is empty
        CurrentSlotNotAvailable = 3,        // Indicates that the current slot is not available
        SelectedSlotNotAvailable = 4,       // Indicates that the selected slot is not available
        SelectedSlotTagMismatch = 5,        // Indicates that the selected slot doesn't match required tags
        ItemsAreSame = 6,                   // Indicates that the selected and current items are the same
        InventoryFull = 7                   // Indicates that the inventory is full
    }
}