using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public class InventoryManagement : MonoBehaviour
    {
        private Dictionary<InventoryType, InventoryController> _inventoryControllers;

        private void Awake()
        {
            _inventoryControllers = new Dictionary<InventoryType, InventoryController>();
        }

        public void AddInventory(InventoryController controller)
        {
            if(_inventoryControllers.ContainsKey(controller.Type) == true) return;
            
            _inventoryControllers.Add(controller.Type, controller);
        }

        public InventoryController GetController(InventoryType type)
        {
            if(_inventoryControllers.ContainsKey(type) == false) return null;
            return _inventoryControllers[type];
        }

        public MoveItemResult MoveItem(ISlotInfo from, ISlotInfo to)
        {
            if (from.InventoryType == to.InventoryType)
            {
                //Inventory A to Inventory A
                return GetController(from.InventoryType).MoveItemTo(from.Item.Name, to.Id);

            }
            else
            {
                //Inventory A to Inventory B
                Debug.LogError("MoveItem Inventory A to Inventory B");
                return MoveItemResult.None;
            }
        }

        public SwapItemResult SwapItem(ISlotInfo from, ISlotInfo to)
        {
            if (from.InventoryType == to.InventoryType)
            {
                //Inventory A to Inventory A
                return GetController(from.InventoryType).SwapItems(from.Item.Name, to.Id);
            }
            else
            {
                Debug.LogError("SwapItem Inventory A to Inventory B");
                //Inventory A to Inventory B
                return SwapItemResult.None;
            }
        }
    }
}