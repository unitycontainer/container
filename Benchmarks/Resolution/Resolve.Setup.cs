using BenchmarkDotNet.Attributes;
using System;
#if NET462
using Microsoft.Practices.Unity;
#else
using Unity.Injection;
using Unity.Lifetime;
using Unity;
#endif

namespace Unity.Benchmarks
{
    public partial class ResolutionBenchmarks
    {
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
                .RegisterType(typeof(TestGeneric<>))
                .RegisterType(typeof(Service), new ContainerControlledLifetimeManager());
#if !NET462 && !NET472
            ContainerAsync = (IUnityContainerAsync)Container;
            ServiceProvider = (IServiceProvider)Container;
#endif

            for (var i = 0; i < 100; i++)
            {
                Container.RegisterInstance(i.ToString(), i);
            }
        }


        public interface IService { }

        public interface IService<T> { }

        public class Service : IService { }
        public class Service<T> : IService<T> { }

        public class TestGeneric<T> { }
    }
}
