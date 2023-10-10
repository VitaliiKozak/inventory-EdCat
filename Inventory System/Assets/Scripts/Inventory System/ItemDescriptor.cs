using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "ItemDescriptor", menuName = "Create/ItemDescriptor")]
    public class ItemDescriptor : ScriptableObject
    {
        [SerializeField] private List<ItemData> _itemDataList = new List<ItemData>();

    }
}