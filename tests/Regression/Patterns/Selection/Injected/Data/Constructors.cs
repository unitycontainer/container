using static Selection.Pattern;

namespace Selection.Injected.Constructors
{
    public class BaselineTestType<TItem1, TItem2>
        : ConstructorSelectionBase
    {
        public BaselineTestType()
            => Data[0] = new object[0];

        public BaselineTestType(TItem1 value) 
            => Data[1] = new object[] { value };

        public BaselineTestType(TItem2 value) 
            => Data[2] = new object[] { value };

        public BaselineTestType(TItem1 item1, TItem2 item2) 
            => Data[3] = new object[] { item1, item2 };
    }

    public class BaselineTestType<TItem1, TItem2, TItem3>
        : ConstructorSelectionBase
    {
        public BaselineTestType()
            => Data[0] = new object[0];


        public BaselineTestType(TItem1 value)
            => Data[1] = new object[] { value };

        public BaselineTestType(TItem2 value)
            => Data[2] = new object[] { value };

        public BaselineTestType(TItem3 value)
            => Data[3] = new object[] { value };


        public BaselineTestType(TItem1 item1, TItem2 item2)
            => Data[4] = new object[] { item1, item2 };

        public BaselineTestType(TItem2 item2, TItem3 item3)
            => Data[5] = new object[] { item2, item3 };

        public BaselineTestType(TItem1 item1, TItem3 item3)
            => Data[6] = new object[] { item1, item3 };


        public BaselineTestType(TItem1 item1, TItem2 item2, TItem3 item3)
            => Data[7] = new object[] { item1, item2, item3 };
    }
}
