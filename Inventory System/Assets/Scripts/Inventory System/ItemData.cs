using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    [System.Serializable]
    public class ItemData
    {
        [field: SerializeField] public ItemName Name { get; private set; }
        [field: SerializeField] public SlotTag SlotsData { get; private set; }//which slot can be placed. Always can place in General slot
        [field: SerializeField] public ItemTag TagData { get; private set; }
        [field: SerializeField] public List<AffectData> AffectDataList { get; private set; }
      

        public string NameKey => Name.ToString() + GlobalConstants.InventoryConstants.NameKeySuffix; 
        public string DescriptionKey => Name.ToString() + GlobalConstants.InventoryConstants.DescriptionNameSuffix;
        public string UiSpriteKey => Name.ToString() + GlobalConstants.InventoryConstants.SpriteNameSuffix;

        public ItemData(ItemName name)
        {
            Name = name;
            AffectDataList = new List<AffectData>();
            SlotsData = SlotTag.Nothing;
            TagData = ItemTag.Nothing;
        }

        public void AddAffect(AffectData data)
        {
            AffectDataList.Add(data);
        }

        public void AddParameter(SlotTag itemSlot)
        {
            SlotsData |= itemSlot;
        }
        
        public void AddParameter(ItemTag itemTag)
        {
            TagData |= itemTag;
        }
    }
}