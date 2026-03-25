using UnityEngine;

namespace REInventory
{
    public interface IStorable
    {
        string Name { get; }
        string Description { get; }
        Sprite Icon { get; }
        int Width { get; }
        int Height { get; }
        IRuntimeStorable GetRuntimeInstance();
    }
}