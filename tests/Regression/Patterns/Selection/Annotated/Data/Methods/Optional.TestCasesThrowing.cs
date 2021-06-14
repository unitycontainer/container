using static Selection.Pattern;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Selection.Annotated.Methods.Optional.TestCasesThrowing
{
    public class OpenGenericType<T>
    {
        [InjectionMethod]
        public void Method([OptionalDependency] T value) { }
    }
}