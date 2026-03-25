using System;

namespace REInventory
{
    public interface IStorableOption
    {
        string OptionLabel { get; }
        void Submit();
        void Setup(Action submitAction, string label);
    }
}