using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    [Flags]
    public enum SlotTag
    {
        Nothing = 0,
        Everything = 1,
        Body = 2,
        Hand = 4,
        Legs = 8,
        Weapon = 16,
        Backpack = 32,
        General = 64,
        Activator = 128,
        Quest1 = 256,
        Quest2 = 512,
    }
    [Flags]
    public enum ItemTag
    {
        Nothing = 0,
        Everything = 1,
        Craft = 2,
        Cooking = 4,
        Ingredient = 8,
        Disassemble = 16,
        Slot = 32,
        Loot = 64,
    }
    
    public enum ItemName
    {
        None,
        LootCrate,
        Carmine,
    }
}