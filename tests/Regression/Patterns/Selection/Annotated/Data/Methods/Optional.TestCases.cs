using static Selection.Pattern;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Selection.Annotated.Methods.Optional.TestCases
{
    public class DynamicParameter : MethodSelectionBase
    {
        [InjectionMethod]
        public void Method([OptionalDependency] dynamic value) => Data[0] = value;
        public override bool IsSuccessful => this[0] is not null;
    }

#if !BEHAVIOR_V4
    public class StructParameter : ConstructorSelectionBase
    {
        [InjectionMethod]
        public void Method([OptionalDependency] TestStruct value) => Data[0] = value;
        public override bool IsSuccessful => this[0] is not null;
    }
#endif
}
