using static Selection.Pattern;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Selection.Annotated.Methods.Required.TestCasesThrowing
{
#if !BEHAVIOR_V4
    public class StructParameter : ConstructorSelectionBase
    {
        [InjectionMethod]
        public void Method([Dependency] TestStruct value) => Data[0] = value;
        public override bool IsSuccessful => this[0] is not null;
    }
#endif

    public class OpenGenericType<T>
    {
        [InjectionMethod]
        public void Method([Dependency] T value) { }
    }
}