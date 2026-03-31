namespace REInventory.Core
{
    public struct GridPosition
    {
        public int X;
        public int Y;

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