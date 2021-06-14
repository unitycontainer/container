using System;
using static Selection.Pattern;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Selection.Annotated.Fields.Required.TestCasesThrowing
{
#if !BEHAVIOR_V4
    public class StructField : FieldSelectionBase
    {
#if !UNITY_V4
        [Dependency]
#endif
        public TestStruct Field;
        public override bool IsSuccessful => this[0] is not null;
    }
#endif

    public class OpenGenericType<T>
    {
#if !UNITY_V4
        [Dependency] 
#endif
        public T Field;
    }
}
