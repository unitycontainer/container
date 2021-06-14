using static Selection.Pattern;
using System;

namespace Selection.Injected.Fields
{
    public class BaselineTestType<TItem1, TItem2>
        : FieldSelectionBase
    {
        public TItem1 Field1;
        public TItem2 Field2;
    }

    public class BaselineTestType<TItem1, TItem2, TItem3>
        : FieldSelectionBase
    {
        public TItem1 Field1;
        public TItem2 Field2;
        public TItem2 Field3;
    }
}
