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
#if NET462 || NET472
            Container = new UnityContainer()
                .RegisterType(typeof(TestGeneric<>))
                .RegisterType(typeof(Service));
#else
            Container = new UnityContainer()
                .RegisterType(typeof(TestGeneric<>))
                .RegisterType(typeof(Service));
            ContainerAsync = (IUnityContainerAsync)Container;
            ServiceProvider = (IServiceProvider)Container;
#endif
        }


        public interface IService { }
        public interface IService<T> { }

        public class Service : IService { }
        public class Service<T> : IService<T> { }

        public class TestGeneric<T> { }
    }
}
