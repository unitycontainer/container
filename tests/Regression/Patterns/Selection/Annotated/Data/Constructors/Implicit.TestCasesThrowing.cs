using Regression;
using static Selection.Pattern;


namespace Selection.Implicit.Constructors.TestCasesThrowing
{
    public class NoPublicCtor : SelectionBaseType 
    {
        private NoPublicCtor() { }
    }

    public class RefParameter : SelectionBaseType
    {
        public RefParameter(ref int value) { }
    }

    public class OutParameter : SelectionBaseType
    {
        public OutParameter(out int value) { value = 0; }
    }

#if !BEHAVIOR_V4
    public class StructParameter : ConstructorSelectionBase
    {
        public StructParameter(TestStruct value) => Data[0] = value;
        public override bool IsSuccessful => this[0] is not null;
    }
#endif
    
    public class OpenGenericType<T>
    {
        public OpenGenericType(T value) { }
    }
}
