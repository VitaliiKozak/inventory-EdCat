using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InventorySystem
{


    public interface ICapacityProvider
    {
        int GetCapacity();
        SimpleEvent<int> OnCapacityChangeEvent { get; }
    }

    public class InfinityCapacityProvider : ICapacityProvider
    {
        public SimpleEvent<int> OnCapacityChangeEvent { get; } = new SimpleEvent<int>();
        
        public int GetCapacity()
        {
            return int.MaxValue;
        }
    }

    public class EquipmentCapacityProvider : ICapacityProvider
    {
        public SimpleEvent<int> OnCapacityChangeEvent { get; } = new SimpleEvent<int>();

        protected ISlotInfo _slotListen;

        protected int _curentCapacity;
        
        public EquipmentCapacityProvider(ISlotInfo slotToListen)
        {
            _slotListen = slotToListen;
            _slotListen.OnInfoChangeEvent.AddListener(ChangeInfoListener);
            ChangeInfoListener(_slotListen);
        }
        public int GetCapacity()
        {
            return _curentCapacity;
        }

        protected virtual void ChangeInfoListener(ISlotInfo slotInfo)
        {
            _curentCapacity = 0;
            if (slotInfo.IsFree == false)
            {
                if (slotInfo.Item.AffectDataList.Any(x => x.Affect == AffectType.BackPackCapacity))
                {
                    _curentCapacity = slotInfo.Item.AffectDataList.Find(x => x.Affect == AffectType.BackPackCapacity).Value;
                }
            }
            OnCapacityChangeEvent?.Notify(_curentCapacity);
        }
    }
}