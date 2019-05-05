namespace Unity.Tests.Override
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