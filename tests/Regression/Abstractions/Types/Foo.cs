using System;
using System.Threading;
#if UNITY_V4
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Regression
{
    public interface IFoo<TEntity>
    {
        TEntity Value { get; }
    }
    public interface IFoo1<TEntity>
    {
        TEntity Value { get; }
    }
    public interface IFoo2<TEntity>
    {
        TEntity Value { get; }
    }
    public class Foo<TEntity> : IFoo<TEntity>, 
                                IFoo1<TEntity>, 
                                IFoo2<TEntity>
    {
        public Foo() { }

        [InjectionConstructor]
        public Foo(TEntity value)
        {
            Value = value;
        }

        public TEntity Value { get; }
    }
}


