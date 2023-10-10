using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public interface ISlotInfo
    {
        int Count { get; }
        bool IsFree { get; }
        bool IsAvailable { get; }
        ItemData Item { get; }
        SlotTag Tags { get; }
        SimpleEvent<ISlotInfo> OnInfoChangeEvent { get; }
    }
    [System.Serializable]
    public class Slot :ISlotInfo
    {
        public int Id { get; private set; }
        public int Count { get; private set; }
        public SlotTag Tags { get; private set; }
        public ItemData Item{ get; private set; }
        public bool IsAvailable{ get; private set; }
        public bool IsFree => Item == null;
        public SimpleEvent<ISlotInfo> OnInfoChangeEvent { get; private set; } = new SimpleEvent<ISlotInfo>();
        
        public Slot(int id, SlotTag tags)
        {
            Id = id;
            Count = 0;
            Tags = tags;
            IsAvailable = false;
            Item = null;
            NotifyCountChange();
        }

        public void SetItem(ItemData data, int count)
        {
            Item = data;
            Count = count;
            NotifyCountChange();
        }
        
        public bool CanGet(int value)
        {
            return Count >= value;
        }

        public void Reduce(int value)
        {
            if (value > Count) throw new Exception($"Not enough Item = {Item.Name}, Current = {Count}, Need = {value}");
            Count -= value;
            CheckItemStock();
            NotifyCountChange();
        }

        public void Add(int value)
        {
            Count += value;
            NotifyCountChange();
        }

        public void SetAvailable(bool isAvailable)
        {
            IsAvailable = isAvailable;
            NotifyCountChange();
        }

        public override string ToString()
        {
            var itemKey = Item == null ? "Free" : Item.Name.ToString();
            return $"[Slot: Id ={Id}, Item = {itemKey}, Count = {Count}, IsAvailable = {IsAvailable}, Tags = {Tags.ToString()}]";
        }

        private void NotifyCountChange()
        {
            OnInfoChangeEvent.Notify(this);
        }
        private void CheckItemStock()
        {
            if (Count <= 0)
            {
                Item = null;
            }
        }
    }
}