using static Selection.Pattern;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Selection.Injected.Methods
{
    public class BaselineTestType<TItem1, TItem2>
        : MethodSelectionBase
    {
        public void Method()
            => Data[0] = new object[0];

        public void Method(TItem1 value)
            => Data[1] = new object[] { value };

        public void Method(TItem2 value)
            => Data[2] = new object[] { value };

        public void Method(TItem1 item1, TItem2 item2)
            => Data[3] = new object[] { item1, item2 };
    }


    public class BaselineTestType<TItem1, TItem2, TItem3>
        : MethodSelectionBase
    {
        public void Method()
            => Data[0] = new object[0];


        public void Method(TItem1 value)
            => Data[1] = new object[] { value };

        public void Method(TItem2 value)
            => Data[2] = new object[] { value };

        public void Method(TItem3 value)
            => Data[3] = new object[] { value };


        public void Method(TItem1 item1, TItem2 item2)
            => Data[4] = new object[] { item1, item2 };

        public void Method(TItem2 item2, TItem3 item3)
            => Data[5] = new object[] { item2, item3 };

        public void Method(TItem1 item1, TItem3 item3)
            => Data[6] = new object[] { item1, item3 };


        public void Method(TItem1 item1, TItem2 item2, TItem3 item3)
            => Data[7] = new object[] { item1, item2, item3 };
    }
}
