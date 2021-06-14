using System;
using static Selection.Pattern;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Selection.Annotated.Properties.Required.TestCasesThrowing
{
#if !BEHAVIOR_V4
    public class StructProperty : PropertySelectionBase
    {
        [Dependency] public TestStruct Property { get; set; }
    }
#endif

    public class OpenGenericType<T>
    {
        [Dependency] public T Property { get; set; }
    }
}
