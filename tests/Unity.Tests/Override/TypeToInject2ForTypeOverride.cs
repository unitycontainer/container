namespace Unity.Tests.v5.Override
{
    public class TypeToInject2ForTypeOverride : IForTypeToInject
    {
        public TypeToInject2ForTypeOverride(int value)
        {
            Value = value;
        }

        public int Value { get; set; }
    }
}