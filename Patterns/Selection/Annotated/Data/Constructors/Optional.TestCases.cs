using static Selection.Pattern;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Selection.Annotated.Constructors.Optional.TestCases
{
    public class DynamicParameter : ConstructorSelectionBase
    {
        public DynamicParameter([OptionalDependency] dynamic value) => Data[0] = value;
        public override bool IsSuccessful => this[0] is not null;
    }

    // TODO: Requires investigation
#if !BEHAVIOR_V4
    public class StructParameter : ConstructorSelectionBase
    {
        public StructParameter([OptionalDependency] TestStruct value) => Data[0] = value;
        public override bool IsSuccessful => this[0] is not null;
    }
#endif
}
