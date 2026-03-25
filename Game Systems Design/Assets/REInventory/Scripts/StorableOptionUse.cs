using System;

namespace REInventory
{
    public class StorableOptionUse : StorableOptionBase
    {
        public override string OptionLabel => "Use"; // TODO: Add localisation support

        public StorableOptionUse(Action submitAction) : base(submitAction) { }
    }
}