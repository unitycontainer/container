using static Selection.Pattern;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Selection.Annotated.Constructors.Required.TestCasesThrowing
{
#if !BEHAVIOR_V4
    public class StructParameter : ConstructorSelectionBase
    {
        public StructParameter([Dependency] TestStruct value) => Data[0] = value;
        public override bool IsSuccessful => this[0] is not null;
    }
#endif

    public class OpenGenericType<T>
    {
        public OpenGenericType([Dependency] T value) { }
    }
}
