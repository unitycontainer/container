using static Selection.Pattern;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif



namespace Selection.Annotated.Constructors.Optional.TestCasesThrowing
{ 
    public class OpenGenericType<T>
    {
        public OpenGenericType([OptionalDependency] T value) { }
    }
}