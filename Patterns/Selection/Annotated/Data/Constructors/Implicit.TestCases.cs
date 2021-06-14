using static Selection.Pattern;


namespace Selection.Implicit.Constructors.TestCases
{
    public class DynamicParameter : ConstructorSelectionBase
    {
        public DynamicParameter(dynamic value) => Data[0] = value;
        public override bool IsSuccessful => this[0] is not null;
    }
}
