using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using Unity.Resolution;
using Unity.Container;
using System.Reflection;
#if NET462
using Microsoft.Practices.Unity;
#else
using Unity.Injection;
using Unity.Lifetime;
using Unity;
#endif

namespace Unity.Benchmarks
{
    public partial class PipelineBenchmarks
    {
        const string EveryTime      = "EveryTime";
        const string OnceInLifetime = "OnceInLifetime";
        const string OnceInAWhile   = "OnceInAWhile";

#if NET462 || NET472
        protected IUnityContainer      Container;
#else
        protected IUnityContainer      Container;
        protected IUnityContainerAsync ContainerAsync;
        protected IServiceProvider     ServiceProvider;
#endif

        [GlobalSetup]
        public virtual void GlobalSetup()
        {
            Container = new UnityContainer()
                .RegisterType(typeof(Service), EveryTime,      new EveryTimeManager())
                .RegisterType(typeof(Service), OnceInLifetime, new OnceInLifetimeManager())
                .RegisterType(typeof(Service), OnceInAWhile,   new OnceInAWhileManager());
#if !NET462 && !NET472
            ContainerAsync = (IUnityContainerAsync)Container;
            ServiceProvider = (IServiceProvider)Container;
#endif
        }


        public interface IService { }

        public class Service : IService { }
        
        public class OtherService : IService { }

        public class CompleteService : IService { }


        public class EveryTimeManager : LifetimeManager, ITypeLifetimeManager
        {
            public override ResolutionStyle Style => ResolutionStyle.EveryTime;

            protected override LifetimeManager OnCreateLifetimeManager() => throw new NotSupportedException();

            public override void SetValue(object newValue, ICollection<IDisposable> lifetime) { }
            public override object TryGetValue(ICollection<IDisposable> lifetime)
            {
                Pipeline = null;
                return NoValue;
            }
            public override object GetValue(ICollection<IDisposable> lifetime)
            {
                Pipeline = null;
                return NoValue;
            }
        }

        public class OnceInLifetimeManager : LifetimeManager, ITypeLifetimeManager
        {
            public override ResolutionStyle Style => ResolutionStyle.OnceInLifetime;

            protected override LifetimeManager OnCreateLifetimeManager() => throw new NotSupportedException();

            public override void SetValue(object newValue, ICollection<IDisposable> lifetime)
            { }
            public override object TryGetValue(ICollection<IDisposable> lifetime)
            {
                Pipeline = null;
                return NoValue;
            }
            public override object GetValue(ICollection<IDisposable> lifetime)
            {
                Pipeline = null;
                return NoValue;
            }
        }

        public class OnceInAWhileManager : LifetimeManager, ITypeLifetimeManager
        {
            public override ResolutionStyle Style => ResolutionStyle.OnceInWhile;

            protected override LifetimeManager OnCreateLifetimeManager() => throw new NotSupportedException();

            public override void SetValue(object newValue, ICollection<IDisposable> lifetime)
            { }
            public override object TryGetValue(ICollection<IDisposable> lifetime)
            {
                Pipeline = null;
                return NoValue;
            }
            public override object GetValue(ICollection<IDisposable> lifetime)
            {
                Pipeline = null;
                return NoValue;
            }
        }
    }
}
