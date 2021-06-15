namespace Unity.Tests.v5.Override
{
    public class TypeToToUndergoeTypeBasedInject2 : IForToUndergoeInject
    {
        public TypeToToUndergoeTypeBasedInject2(TypeToInject2ForTypeOverride injectedObject)
        {
            IForTypeToInject = injectedObject;
        }

        public IForTypeToInject IForTypeToInject { get; set; }
        public TypeToInject2ForTypeOverride TypeToInject2ForTypeOverride { get; set; }
    }
}