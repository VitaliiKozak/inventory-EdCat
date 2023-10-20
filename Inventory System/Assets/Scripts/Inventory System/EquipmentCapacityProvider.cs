using System.Linq;

namespace InventorySystem
{
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