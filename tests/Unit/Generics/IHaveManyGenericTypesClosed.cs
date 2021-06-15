namespace Unity.Tests.v5.Generics
{
    public interface IHaveManyGenericTypesClosed
    {
        GenericA PropT1 { get; set; }
        GenericB PropT2 { get; set; }
        GenericC PropT3 { get; set; }
        GenericD PropT4 { get; set; }

        void Set(GenericA value);
        void Set(GenericB value);
        void Set(GenericC value);
        void Set(GenericD value);

        void SetMultiple(GenericD t4Value, GenericC t3Value);
    }
}