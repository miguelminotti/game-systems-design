using System;

namespace REInventory
{
    public class StorableOptionDrop : StorableOptionBase
    {
        public override string OptionLabel => "Drop"; // TODO: Add localisation support

        public StorableOptionDrop(Action submitAction) : base(submitAction) { }
    }
}