
namespace Unity.Tests.v5.Generics
{
    public class Foo<TEntity> : IFoo<TEntity>
    {
        public Foo()
        {
        }

        public Foo(TEntity value)
        {
            Value = value;
        }

        public TEntity Value { get; }
    }

    public class Foo : IFoo
    {
    }
}