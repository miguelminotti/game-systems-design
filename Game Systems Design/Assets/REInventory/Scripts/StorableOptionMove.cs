using System;

namespace REInventory
{
    public class StorableOptionMove : StorableOptionBase
    {
        public override string OptionLabel => "Move"; // TODO: Add localisation support

        public StorableOptionMove(Action submitAction) : base(submitAction) { }
    }
}