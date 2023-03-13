namespace Unit.Test.Generics
{
    public interface IHaveAGenericType<T1>
    {
        T1 PropT1 { get; set; }

        void Set(T1 value);
    }
}