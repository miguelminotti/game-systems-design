using System;

namespace REInventory
{
    public interface IInventoryGrid
    {
        int MaxCapacity { get; }
        int Width { get; }
        int Height { get; }

        IInventoryGridSlot GetSlot(int x, int y);
        bool PlaceItem(IRuntimeStorable item, int x, int y);
        bool PlaceItemOnAvailableSpace(IRuntimeStorable item);
        bool RemoveItem(IRuntimeStorable item);
        bool IsPlaceableAt(int x, int y, int itemWidth, int itemHeight);
    }

    public class InventoryGrid : IInventoryGrid
    {
        public int MaxCapacity { get; }
        public int Width { get; }
        public int Height { get; }

        private readonly InventoryGridSlot[,] _gridSlots;

        public InventoryGrid(int width, int height)
        {
            Width = width;
            Height = height;
            MaxCapacity = width * height;

            _gridSlots = new InventoryGridSlot[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _gridSlots[x, y] = new InventoryGridSlot(x, y);
                }
            }
        }

        public IInventoryGridSlot GetSlot(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException("Slot coordinates are out of bounds.");
            return _gridSlots[x, y];
        }

        public bool PlaceItem(IRuntimeStorable item, int x, int y)
        {
            int itemWidth = item.BaseItem.Width;
            int itemHeight = item.BaseItem.Height;
            if (!IsPlaceableAt(x, y, itemWidth, itemHeight))
            {
                return false;
            }

            item.SetPosition(x, y);
            FillSlotsForItem(item, x, y);
            return true;
        }

        public bool PlaceItemOnAvailableSpace(IRuntimeStorable item)
        {
            var slotToFill = new { X = -1, Y = -1, Found = false };

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (!GetSlot(x, y).IsOccupied() && IsPlaceableAt(x, y, item.BaseItem.Width, item.BaseItem.Height))
                    {
                        slotToFill = new { X = x, Y = y, Found = true };
                        item.SetPosition(x, y);
                        break;
                    }
                }
            }

            if (slotToFill.Found)
            {
                FillSlotsForItem(item, slotToFill.X, slotToFill.Y);
                return true;
            }

            return false; // No available space found for the item
        }

        public bool RemoveItem(IRuntimeStorable item)
        {
            bool itemFound = false;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (GetSlot(x, y).StoredItem == item)
                    {
                        GetSlot(x, y).RemoveItemStored();
                        itemFound = true;
                    }
                }
            }
            return itemFound;
        }

        public bool IsPlaceableAt(int x, int y, int itemWidth, int itemHeight)
        {
            // Change this to check if there's enough space for the item and if the slots are empty
            for (int i = 0; i < itemWidth; i++)
            {
                for (int j = 0; j < itemHeight; j++)
                {
                    int checkX = x + i;
                    int checkY = y + j;
                    if (checkX >= Width || checkY >= Height || GetSlot(checkX, checkY).IsOccupied())
                    {
                        return false; // Not placeable if out of bounds or slot is occupied
                    }
                }
            }
            return true; // Placeable if all slots are within bounds and unoccupied
        }

        private void FillSlotsForItem(IRuntimeStorable item, int x, int y)
        {
            for (int i = 0; i < item.BaseItem.Width; i++)
            {
                for (int j = 0; j < item.BaseItem.Height; j++)
                {
                    GetSlot(x + i, y + j).StoreItem(item);
                }
            }
        }
    }
}