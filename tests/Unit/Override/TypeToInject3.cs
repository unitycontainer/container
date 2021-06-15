namespace Unity.Tests.v5.Override
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