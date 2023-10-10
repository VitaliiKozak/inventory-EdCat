using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public class InventoryController
    {
        public InventoryType Type => _inventoryDescriptor.Type;
        public Inventory  Inventory => _inventory;
        
        private readonly  ItemsDataRepository _itemsDataRepository;
        private readonly Inventory _inventory;
        private readonly InventoryDescriptor _inventoryDescriptor;
        
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
                    _inventory.AddSlot(new Slot(index +slotInitData.AditionalIndex, slotInitData.Tags));
                    index++;
                }
            }
        }
    }

    public enum InventoryType
    {
        None,
        Player,
        Stock,
    }

    [System.Serializable]
    public struct SlotInitData
    {
        public int AditionalIndex;
        public int Count;
        public SlotTag Tags;
    }
    
}
