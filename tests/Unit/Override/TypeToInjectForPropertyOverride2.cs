namespace Unit.Test.Override
{
    public class TypeToInjectForPropertyOverride2 : IInterfaceForTypesToInjectForPropertyOverride
    {
        public TypeToInjectForPropertyOverride2(int value)
        {
            Value = value;
        }
        public int Value { get; set; }
    }
}