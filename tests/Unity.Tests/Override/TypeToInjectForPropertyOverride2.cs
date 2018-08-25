namespace Unity.Tests.v5.Override
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