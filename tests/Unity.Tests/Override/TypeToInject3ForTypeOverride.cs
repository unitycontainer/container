namespace Unity.Tests.Override
{
    public class TypeToInject3ForTypeOverride : IForTypeToInject
    {
        public TypeToInject3ForTypeOverride(int value)
        {
            Value = value;
        }

        public int Value { get; set; }
        public string PropertyToInject { get; set; }
    }
}