
using UnityEngine;

namespace InventorySystem
{
    //not move in file with enums, because this from project EdCat
    public enum AffectType
    {
        Satiety,
        Health,
        Speed,
        Armor,
        MeleeDamage,
        RangedDamage,
        BackPackCapacity,
        Bonus,
    }


    [System.Serializable]
    public struct AffectData
    {
        [field: SerializeField] public AffectType Affect { get; private set; }
        [field: SerializeField] public int Value { get; private set; }

        public AffectData(AffectType affect, int valueData)
        {
            Affect = affect;
            Value = valueData;
        }
    }
}