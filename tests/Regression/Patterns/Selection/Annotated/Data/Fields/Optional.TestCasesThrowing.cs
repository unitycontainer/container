using static Selection.Pattern;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Selection.Annotated.Fields.Optional.TestCasesThrowing
{

    public class OpenGenericType<T>
    {
#if !UNITY_V4
        [OptionalDependency] 
#endif
        public T Field;
    }
}
