using static Selection.Pattern;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Selection.Implicit.Methods.TestCasesThrowing
{
    public class RefParameter : SelectionBaseType
    {
        [InjectionMethod]
        public void Method(ref int value) { }
    }

    public class OutParameter : SelectionBaseType
    {
        [InjectionMethod]
        public void Method(out int value) { value = 0; }
    }

#if !BEHAVIOR_V4
    public class StructParameter : ConstructorSelectionBase
    {
        [InjectionMethod]
        public void Method(TestStruct value) => Data[0] = value;
        public override bool IsSuccessful => this[0] is not null;
    }
#endif
    
    public class OpenGenericType<T>
    {
        [InjectionMethod]
        public void Method(T value) { }
    }
}
