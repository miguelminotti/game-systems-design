namespace REInventory.Core
{
    public interface IStorable
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        int Width { get; }
        int Height { get; }
        IRuntimeStorable GetRuntimeInstance();
    }
}