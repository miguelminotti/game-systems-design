namespace REInventory.Core
{
    public readonly struct GridPosition
    {
        public int X { get; }
        public int Y { get; }

        public GridPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public readonly GridPosition Move(int x, int y)
        {
            return new GridPosition(X + x, Y + y);
        }
    }
}