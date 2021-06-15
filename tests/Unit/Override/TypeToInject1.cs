namespace Unity.Tests.v5.Override
{
    public class TypeToInject1 : IInterfaceForTypesToInject
    {
        public TypeToInject1(int value)
        {
            Value = value;
        }
        public int Value { get; set; }
    }
}