using System;

namespace REInventory.Core
{
    public class StorableOptionBase : IStorableOption
    {
        public virtual string OptionLabel => _label;

        private Action _submitAction;
        private string _label;

        public StorableOptionBase(Action submitAction, string label)
        {
            _submitAction = submitAction;
            _label = label;
        }

        public virtual void Submit()
        {
            _submitAction?.Invoke();
        }

        public void Setup(Action submitAction, string label)
        {
            _submitAction = submitAction;
            _label = label;
        }
    }
}