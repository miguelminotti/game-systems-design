using System;

namespace REInventory
{
    public abstract class StorableOptionBase : IStorableOption
    {
        public abstract string OptionLabel { get; } // Placeholder label

        private Action _submitAction;

        public StorableOptionBase(Action submitAction)
        {
            _submitAction = submitAction;
        }

        public virtual void Submit()
        {
            _submitAction?.Invoke();
        }
    }
}