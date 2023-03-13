namespace Unit.Test.Override
{
    public class TypeToInject3 : IInterfaceForTypesToInject
    {
        public TypeToInject3(int value)
        {
            Value = value;
        }
        public int Value { get; set; }
    }
}