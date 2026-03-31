namespace REInventory.Core
{
	public static class InventoryEvents
	{
        public interface IInventoryChangedEvent
        {
            IInventoryCore RefInventory { get; }
            IRuntimeStorable Item { get; }
            InventoryChangeType ChangeType { get; }
        }

        public enum InventoryChangeType
        {
            Added,
            Removed,
            Rotated
        }

        public interface IInventoryClearedEvent
        {
            IInventoryCore RefInventory { get; }
        }
    }
}