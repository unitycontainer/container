namespace Unity.Tests.v5.Override
{
    public class TypeToInject1ForTypeOverride : IForTypeToInject
    {
        public TypeToInject1ForTypeOverride(int value)
        {
            Value = value;
        }

        public int Value { get; set; }
    }
}