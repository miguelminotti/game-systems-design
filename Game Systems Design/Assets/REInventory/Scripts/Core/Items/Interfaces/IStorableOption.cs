using System;

namespace REInventory.Core
{
    public interface IStorableOption
    {
        string OptionLabel { get; }
        void Submit();
        void Setup(Action submitAction, string label);
    }
}