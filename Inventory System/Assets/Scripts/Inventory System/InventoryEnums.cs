using System;

namespace  InventorySystem
{
    public enum InventoryType
    {
        None,
        Player,
        Stock,
        Equipment,
    }
    
    public enum EquipItemResult
    {
        None,                       // No issues
        Success,                    // Indicates success
        CurrentSlotEmpty = 2,                // Indicates that the current slot is empty
        CurrentSlotNotAvailable = 3,        // Indicates that the current slot is not available
        SelectedSlotNotAvailable = 4,       // Indicates that the selected slot is not available
        SelectedSlotTagMismatch = 5,        // Indicates that the selected slot doesn't match required tags
        ItemsAreSame = 6,                   // Indicates that the selected and current items are the same
        InventoryFull = 7                   // Indicates that the inventory is full
    }
    public enum ItemAddResult
    {
        None,                         // No issues
        AbsenceOfEmptySlots,          // Indicates the absence of empty slots
        AbsenceOfSlotsWithSuitableTag,// Indicates the absence of slots with a suitable tag
        Success,                      // Indicates success
    }

    public enum ItemReduceResult
    {
        None,                   // No issues
        Success,                // Indicates success
        AbsenceOfItem,          // Indicates the absence of the item
        InsufficientQuantity,   // Indicates insufficient quantity of the item
    }

    public enum MoveItemResult
    {
        None,                       // No issues
        Success,                    // Indicates success
        NoSuchItem,                 // Indicates that there is no such item
        NoSelectedSlot,             // Indicates that no slot is selected
        SelectedSlotNotAvailable,   // Indicates that the selected slot is not available
        SelectedSlotOccupied,       // Indicates that the selected slot is already occupied
        CurrentSlotMatchesSelected, // Indicates that the current slot matches the selected slot
        SelectedSlotMissingTag,     // Indicates that the selected slot doesn't have the required tag
    }

    public enum SwapItemResult
    {
        None,                       // No issues
        Success,                    // Indicates success
        NoSuchItem,                 // Indicates that there is no such item
        NoSelectedSlot,             // Indicates that no slot is selected
        SelectedSlotNotAvailable,   // Indicates that the selected slot is not available
        SelectedSlotEmpty,          // Indicates that the selected slot is empty
        CurrentSlotMatchesSelected, // Indicates that the current slot matches the selected slot
        SelectedSlotTagMismatch,    // Indicates that the selected slot doesn't match required tags
        CurrentSlotTagMismatch,      // Indicates that the current slot doesn't match required tags
    }
    
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
        Backpack_1lvl,
        Backpack_2lvl,
        Backpack_3lvl,
        Tomato,
        Potato,
        Weapon_TypeA,
        Weapon_TypeB,
        
    }
}
