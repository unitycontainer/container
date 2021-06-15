using static Selection.Pattern;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Selection.Annotated.Methods.Required.TestCases
{
    public class DynamicParameter : MethodSelectionBase
    {
        [InjectionMethod]
        public void Method([Dependency] dynamic value) => Data[0] = value;
        public override bool IsSuccessful => this[0] is not null;
    }

}
