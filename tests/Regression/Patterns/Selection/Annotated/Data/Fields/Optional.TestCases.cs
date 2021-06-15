using static Selection.Pattern;
using System;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Selection.Annotated.Fields.Optional.TestCases
{
#if !UNITY_V4 && !BEHAVIOR_V4
    public class StructField : FieldSelectionBase
    {
        [OptionalDependency] public TestStruct Field;
        public override bool IsSuccessful => this[0] is not null;
    }
#endif
}
