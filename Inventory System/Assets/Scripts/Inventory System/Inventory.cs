using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace InventorySystem
{

    [Serializable]
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
            //repeating the sorting until there is no passage without changes
            do
            {
                makeChanges = false;
                // Iterate through the Slots in reverse order
                for (int i = Slots.Count-1; i >= 0; i--)
                {
                    // Check if the current slot is free; if it's free, skip to the next iteration
                    if(Slots[i].IsFree == true) continue;
                    // Find a suitable slot that is free, available, and compatible with the item in the current slot
                    var slotFree = GetSlot(x => x.IsFree == true&& x.IsAvailable == true && (x.Tags & Slots[i].Item.SlotsData) != SlotTag.Nothing);
                    // If no suitable slot is found or the found slot has a higher ID, skip to the next iteration
                    if(slotFree == null || slotFree.Id > Slots[i].Id) continue;
                    // Set the item in the found slot and reduce the count in the current slot
                    slotFree.SetItem(Slots[i].Item, Slots[i].Count);
                    Slots[i].Reduce(Slots[i].Count);
                    // Mark that changes have been made during this pass
                    makeChanges = true;
                }
            } while (makeChanges);
        }

        public List<Slot> GetInvalidSlots()
        {
            var invalidSlots = new List<Slot>();

            foreach (var slot in Slots)
            {
                if(slot.IsFree == true) continue;
                
                if (slot.IsAvailable == false ||
                    (slot.Item.SlotsData & slot.Tags) == SlotTag.Nothing||
                    slot.Count <= 0)
                {
                    invalidSlots.Add(slot);
                }
            }
            
            return invalidSlots;
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
}