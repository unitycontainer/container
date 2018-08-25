namespace Unity.Tests.v5.Override
{
    public class TypeToInject2 : IInterfaceForTypesToInject
    {
        public TypeToInject2(int value)
        {
            Value = value;
        }
        public int Value { get; set; }
    }
}