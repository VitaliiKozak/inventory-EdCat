using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
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