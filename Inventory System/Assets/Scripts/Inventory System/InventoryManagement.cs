using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public class InventoryManagement : MonoBehaviour
    {
        private Dictionary<InventoryType, Inventory> _inventoryControllers;

        private void Awake()
        {
            _inventoryControllers = new Dictionary<InventoryType, Inventory>();
        }

        public void AddInventory(Inventory controller)
        {
            if(_inventoryControllers.ContainsKey(controller.Type) == true) return;
            
            _inventoryControllers.Add(controller.Type, controller);
        }

        public Inventory GetController(InventoryType type)
        {
            if(_inventoryControllers.ContainsKey(type) == false) return null;
            return _inventoryControllers[type];
        }

        public MoveItemResult MoveItem(ISlotInfo from, ISlotInfo to)
        {
            if (from.InventoryType == to.InventoryType)
            {
                return GetController(from.InventoryType).MoveItemTo(from.Item.Name, to.Id);
            }
            else
            {
                if (to.IsFree == false)
                {
                    switch (SwapItem(from, to))
                    {
                        case SwapItemResult.Success:
                            return MoveItemResult.Success;
                        default:
                            return MoveItemResult.None;
                    }
                }
                var toController = GetController(to.InventoryType);
                switch (toController.CanAddItem(from.Item.Name, from.Count))
                {
                    case ItemAddResult.AbsenceOfEmptySlots:
                        break;
                    case ItemAddResult.AbsenceOfSlotsWithSuitableTag:
                        break;
                    case ItemAddResult.Success:
                    {
                        var fromController = GetController(from.InventoryType);

                        var itemName = from.Item.Name;
                        var itemCount = from.Count;
                        var moveItem = toController.HasItem(itemName) == false;
                        fromController.ReduceItem(itemName, itemCount);
                        toController.AddItem(itemName, itemCount);
                        if (moveItem == true)  toController.MoveItemTo(itemName, to.Id);
                        return MoveItemResult.Success;
                    }
                }
                return MoveItemResult.None;
            }
        }

        public EquipItemResult EquipItem(ISlotInfo from, ISlotInfo to)
        {
            if (from.IsFree == true) return EquipItemResult.None;
            if (from.IsAvailable == false) return EquipItemResult.None;
            if (to.IsAvailable == false) return EquipItemResult.None;

            if((to.Tags & from.Item.SlotsData) == SlotTag.Nothing) return EquipItemResult.None;
            
            var toController = GetController(to.InventoryType);
            var fromController = GetController(from.InventoryType);
            
            if (to.IsFree == true)
            {
                //just equip
                toController.GetSlot(to.Id).SetItem(from.Item,1);
                fromController.GetSlot(from.Id).Reduce(1);
                
                return EquipItemResult.Success;
            }
            else
            {
                if (fromController.HasSlot(x => x.IsAvailable == true && x.IsFree == true && (x.Tags & to.Item.SlotsData) != SlotTag.Nothing) == false)
                {
                    if (from.Count == 1)
                    {
                        if ((from.Tags & to.Item.SlotsData) != SlotTag.Nothing)
                        {
                            //put where we take

                            var fromData = from.Item;
                            var fromCount = from.Count;
                            var fromSlot = fromController.GetSlot(from.Id);
                            fromSlot.SetItem(to.Item, to.Count);
                            var toSlot = toController.GetSlot(to.Id);
                            
                            toSlot.SetItem(fromData, fromCount);
                            return EquipItemResult.Success;
                        }
                        else
                        {
                            return EquipItemResult.None;
                        }
                    }
                    else
                    {
                        return EquipItemResult.None;
                    }
                }
                
               var slot = fromController.GetSlot(x => x.IsAvailable == true && x.IsFree == true && (x.Tags & to.Item.SlotsData) != SlotTag.Nothing);
               
               slot.SetItem(to.Item, to.Count);
               toController.GetSlot(to.Id).SetItem(from.Item,1);
               fromController.ReduceItem(from.Item.Name, 1);
               
               return EquipItemResult.Success;
            }
        }

        public SwapItemResult SwapItem(ISlotInfo from, ISlotInfo to)
        {
            if (from.InventoryType == to.InventoryType)
            {
                return GetController(from.InventoryType).SwapItems(from.Item.Name, to.Id);
            }
            else
            {
                if (to.IsAvailable == false || from.IsAvailable == false) return SwapItemResult.SelectedSlotNotAvailable;
                if (to.IsFree == true ||from.IsFree == true ) return SwapItemResult.SelectedSlotEmpty;

                if ((from.Tags & to.Item.SlotsData) == SlotTag.Nothing) return SwapItemResult.SelectedSlotTagMismatch;
                if ((to.Tags & from.Item.SlotsData) == SlotTag.Nothing) return SwapItemResult.CurrentSlotTagMismatch;

                var fromController = GetController(from.InventoryType);
                var toController = GetController(to.InventoryType);
                
                var countFrom = from.Count;
                var dataFrom = from.Item;

                var countTo = to.Count;
                var dataTo = to.Item;

                if (dataFrom.Name == dataTo.Name)
                {
                    var slotFrom = fromController.GetSlot(from.Item.Name);
                    slotFrom.Reduce(countFrom);
                    var slotTo = toController.GetSlot(to.Item.Name);
                    slotTo.Add(countFrom);
                    return SwapItemResult.Success;
                }
                if (fromController.HasItem(to.Item.Name) == true)
                {
                    var slot = fromController.GetSlot(to.Item.Name);
                    countTo += slot.Count;
                    slot.Reduce(slot.Count);
                }
                if (toController.HasItem(from.Item.Name) == true)
                {
                    var slot = toController.GetSlot(from.Item.Name);
                    countFrom += slot.Count;
                    slot.Reduce(slot.Count);
                }
                fromController.GetSlot(from.Id).SetItem(dataTo,countTo);
                toController.GetSlot(to.Id).SetItem(dataFrom,countFrom);
                return SwapItemResult.Success;
            }
        }
    }
}