namespace InventorySystem
{
    public class InfinityCapacityProvider : ICapacityProvider
    {
        public SimpleEvent<int> OnCapacityChangeEvent { get; } = new SimpleEvent<int>();
        
        public int GetCapacity()
        {
            return int.MaxValue;
        }
    }
}