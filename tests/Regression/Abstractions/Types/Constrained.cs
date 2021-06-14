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
    public interface IConstrained<TEntity>
        where TEntity : IService
    {
        TEntity Value { get; }
    }

    public class Constrained<TEntity> : IConstrained<TEntity>
        where TEntity : Service
    {
        public Constrained()
        {
        }

        public Constrained(TEntity value)
        {
            Value = value;
        }

        public TEntity Value { get; }
    }
}


