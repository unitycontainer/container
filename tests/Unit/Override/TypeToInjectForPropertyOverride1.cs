namespace Unity.Tests.v5.Override
{
    public class TypeToInjectForPropertyOverride1 : IInterfaceForTypesToInjectForPropertyOverride
    {
        public TypeToInjectForPropertyOverride1(int value)
        {
            Value = value;
        }
        public int Value { get; set; }
    }
}