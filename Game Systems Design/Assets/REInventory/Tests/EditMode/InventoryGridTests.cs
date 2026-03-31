using NUnit.Framework;
using REInventory.Core;

namespace REInventory.Tests
{
    public class InventoryGridTests
    {
        [Test]
        public void PlaceItem_ValidPosition_ShouldSucceed()
        {
            // Arrange
            var grid = new InventoryGrid(5, 5);
            var item = new TestItem(2, 2);
            var position = new GridPosition(0, 0);

            // Act
            var result = grid.PlaceItem(item, position);

            // Assert
            Assert.AreEqual(IInventoryGrid.PlaceItemResult.Succeeded, result);
        }

        [Test]
        public void PlaceItem_OutOfBounds_ShouldFail()
        {
            var grid = new InventoryGrid(5, 5);
            var item = new TestItem(3, 3);

            var result = grid.PlaceItem(item, new GridPosition(4, 4));

            Assert.AreEqual(IInventoryGrid.PlaceItemResult.FailedOutOfBounds, result);
        }

        [Test]
        public void PlaceItem_OnOccupiedSpace_ShouldFail()
        {
            var grid = new InventoryGrid(5, 5);

            var item1 = new TestItem(2, 2);
            var item2 = new TestItem(2, 2);

            grid.PlaceItem(item1, new GridPosition(0, 0));

            var result = grid.PlaceItem(item2, new GridPosition(0, 0));

            Assert.AreEqual(IInventoryGrid.PlaceItemResult.FailedOccupied, result);
        }

        [Test]
        public void RemoveItem_ShouldFreeSlots()
        {
            var grid = new InventoryGrid(5, 5);
            var item = new TestItem(2, 2);

            grid.PlaceItem(item, new GridPosition(0, 0));

            bool removed = grid.TryRemoveItem(item);

            Assert.IsTrue(removed);

            var result = grid.PlaceItem(new TestItem(2, 2), new GridPosition(0, 0));
            Assert.AreEqual(IInventoryGrid.PlaceItemResult.Succeeded, result);
        }

        [Test]
        public void TryGetSlot_ValidPosition_ShouldReturnSlot()
        {
            var grid = new InventoryGrid(5, 5);

            bool success = grid.TryGetSlot(new GridPosition(2, 2), out var slot);

            Assert.IsTrue(success);
            Assert.IsNotNull(slot);
        }

        [Test]
        public void PlaceItem_ShouldOccupyAllExpectedSlots()
        {
            var grid = new InventoryGrid(5, 5);
            var item = new TestItem(2, 2);

            grid.PlaceItem(item, new GridPosition(1, 1));

            for (int x = 1; x <= 2; x++)
            {
                for (int y = 1; y <= 2; y++)
                {
                    grid.TryGetSlot(new GridPosition(x, y), out var slot);
                    Assert.AreEqual(item, slot.StoredItem);
                }
            }
        }

        [Test]
        public void TryPlaceItemOnAvailableSpace_ShouldFindFirstAvailable()
        {
            var grid = new InventoryGrid(2, 2);

            grid.PlaceItem(new TestItem(1, 1), new GridPosition(0, 0));

            var newItem = new TestItem(1, 1);

            bool success = grid.TryPlaceItemOnAvailableSpace(newItem);

            Assert.IsTrue(success);
        }
    }
}