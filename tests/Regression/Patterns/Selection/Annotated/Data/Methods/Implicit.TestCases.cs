using static Selection.Pattern;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Selection.Implicit.Methods.TestCases
{
    public class DynamicParameter : MethodSelectionBase
    {
        [InjectionMethod]
        public void Method(dynamic value) => Data[0] = value;
        public override bool IsSuccessful => this[0] is not null;
    }
}
