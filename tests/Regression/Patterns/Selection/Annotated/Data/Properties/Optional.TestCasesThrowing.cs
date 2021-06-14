using System;
using static Selection.Pattern;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Selection.Annotated.Properties.Optional.TestCasesThrowing
{

    public class OpenGenericType<T>
    {
        [OptionalDependency] public T Property { get; set; }
    }
}
