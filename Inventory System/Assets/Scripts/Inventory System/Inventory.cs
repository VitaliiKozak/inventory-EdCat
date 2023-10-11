using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace InventorySystem
{
    public interface IInventory
    {
        bool HasItem(ItemName name);
        bool HasFreeSlot();
        ItemAddResult AddItem(ItemName itemName, int count);
        ItemReduceResult ReduceItem(ItemName itemName, int count);
        MoveItemResult MoveItemTo(ItemName itemName, int slotId);
        SwapItemResult SwapItems(ItemName itemName, int slotId);
    }

    [System.Serializable]
    public class Inventory : IInventory
    {
        public List<Slot> Slots { get; private set; }
        private readonly IItemsDataRepository _repository;

        public Inventory(IItemsDataRepository repository)
        {
            _repository = repository;
            Slots = new List<Slot>();
        }
        public void AddSlot(Slot slot)
        {
            Slots.Add(slot);
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
                Slots.Find(x =>x.IsFree == false &&  x.Item.Name == itemName).Add(count);
                return ItemAddResult.Success;
            }

            if (HasFreeSlot() == false) return ItemAddResult.AbsenceOfEmptySlots;
            var data = _repository.GetItemData(itemName);
            var slot = Slots.Find(x => x.IsFree && x.IsAvailable && x.Tags.HasFlag(data.SlotsData));

            if (slot == null) return ItemAddResult.AbsenceOfSlotsWithSuitableTag;
            
            slot.SetItem(data,count);
            return ItemAddResult.Success;
        }

        public ItemReduceResult ReduceItem(ItemName itemName, int count)
        {
            if (HasItem(itemName) == false) return ItemReduceResult.AbsenceOfItem;

            var slot = Slots.Find(x => x.IsFree == false && x.Item.Name == itemName);
            
            if(slot.CanGet(count) == false) return ItemReduceResult.InsufficientQuantity;

            slot.Reduce(count);
            
            return ItemReduceResult.Success;
        }
        //inner inventory
        public MoveItemResult MoveItemTo(ItemName itemName, int slotId)
        {
            if (HasItem(itemName) == false) return MoveItemResult.NoSuchItem;
            
            var preferSlot = Slots.Find(x => x.Id == slotId);
            if(preferSlot == null) return MoveItemResult.NoSelectedSlot;
            
            if(preferSlot.IsAvailable == false)return MoveItemResult.SelectedSlotNotAvailable;
            if(preferSlot.IsFree == false)return MoveItemResult.SelectedSlotOccupied;
            
            var currentSlot = Slots.Find(x => x.IsFree == false && x.Item.Name == itemName);
            if(preferSlot.Id == currentSlot.Id ) return MoveItemResult.CurrentSlotMatchesSelected;

            if(preferSlot.Tags.HasFlag(currentSlot.Item.SlotsData) == false)  return MoveItemResult.SelectedSlotMissingTag;

            var count = currentSlot.Count;
            var data = currentSlot.Item;
            currentSlot.Reduce(count);
            
            preferSlot.SetItem(data, count);
            
            return MoveItemResult.Success;
        }


        //inner inventory
        public SwapItemResult SwapItems(ItemName itemName, int slotId)
        {
            if (HasItem(itemName) == false) return SwapItemResult.NoSuchItem;

            var preferSlot = Slots.Find(x => x.Id == slotId);
            if(preferSlot == null) return SwapItemResult.NoSelectedSlot;
            
            if(preferSlot.IsAvailable == false) return SwapItemResult.SelectedSlotNotAvailable;
            if(preferSlot.IsFree == true) return SwapItemResult.SelectedSlotEmpty;
            
            var currentSlot = Slots.Find(x => x.IsFree == false && x.Item.Name == itemName);
            if(preferSlot.Id == currentSlot.Id ) return SwapItemResult.CurrentSlotMatchesSelected;
            
            if(preferSlot.Tags.HasFlag(currentSlot.Item.SlotsData) == false) return SwapItemResult.SelectedSlotTagMismatch;
            if(currentSlot.Tags.HasFlag(preferSlot.Item.SlotsData) == false) return SwapItemResult.CurrentSlotTagMismatch;
            
            var count = currentSlot.Count;
            var data = currentSlot.Item;
            
            currentSlot.SetItem(preferSlot.Item, preferSlot.Count);
            preferSlot.SetItem(data, count);
            
            return SwapItemResult.Success;
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
}