using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "InventoryDescriptor", menuName = "Create/InventoryDescriptor")]
    public class InventoryDescriptor : ScriptableObject
    {
        public InventoryType Type;
        public int BaseAvailableSlots;
        public List<SlotInitData> SlotsToInit;
    }
}