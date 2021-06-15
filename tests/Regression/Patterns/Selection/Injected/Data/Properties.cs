using static Selection.Pattern;

namespace Selection.Injected.Properties
{
    public class BaselineTestType<TItem1, TItem2>
        : PropertySelectionBase
    {
        public TItem1 Property1 { get => (TItem1)Data[0]; set => Data[0] = value; }
        public TItem2 Property2 { get => (TItem2)Data[1]; set => Data[1] = value; }
    }

    public class BaselineTestType<TItem1, TItem2, TItem3>
        : PropertySelectionBase
    {
        public TItem1 Property1 { get => (TItem1)Data[0]; set => Data[0] = value; }
        public TItem2 Property2 { get => (TItem2)Data[1]; set => Data[1] = value; }
        public TItem3 Property3 { get => (TItem3)Data[2]; set => Data[2] = value; }
    }
}

