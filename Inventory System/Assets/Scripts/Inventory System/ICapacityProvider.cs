using System.Collections;
using System.Collections.Generic;
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
}