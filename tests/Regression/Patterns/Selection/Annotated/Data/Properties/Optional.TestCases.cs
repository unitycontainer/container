using System;
using static Selection.Pattern;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Selection.Annotated.Properties.Optional.TestCases
{
#if !BEHAVIOR_V4
    public class StructProperty : PropertySelectionBase
    {
        [OptionalDependency] public TestStruct  Property { get; set; }
    }
#endif
}
