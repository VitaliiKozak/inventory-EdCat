using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public interface IItemsDataRepository
    {
        ItemData GetItemData(ItemName name);
    }
    
    public class ItemsDataRepository : MonoBehaviour,IItemsDataRepository
    {
        [SerializeField] private List<ItemData> _itemsData;
        [SerializeField] private ItemData _defaulteData;

        public ItemData GetItemData(ItemName name)
        {
            var result = _itemsData.Find(x => x.Name == name);

            if (result == null)
            {
                result = _defaulteData;
            }

            return result;
        }
        
    }
}