using static Selection.Pattern;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Selection.Annotated.Constructors.Required.TestCases
{
    public class DynamicParameter : ConstructorSelectionBase
    {
        public DynamicParameter([Dependency] dynamic value) => Data[0] = value;
        public override bool IsSuccessful => this[0] is not null;
    }
}

