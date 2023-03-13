namespace Unit.Test.Generics
{
    public interface IFoo<TEntity>
    {
        TEntity Value { get; }
    }

    public interface IFoo
    {
    }
}