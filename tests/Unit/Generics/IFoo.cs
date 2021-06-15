namespace Unity.Tests.v5.Generics
{
    public interface IFoo<TEntity>
    {
        TEntity Value { get; }
    }

    public interface IFoo
    {
    }
}