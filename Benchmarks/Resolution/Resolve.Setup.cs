using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
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
                .RegisterType(typeof(List<>), new InjectionConstructor())
                .RegisterType(typeof(Service))
                .CreateChildContainer();
#else
            Container = new UnityContainer()
                .RegisterType(typeof(List<>), new InjectionConstructor())
                .RegisterType(typeof(Service))
                .CreateChildContainer();
            ContainerAsync = (IUnityContainerAsync)Container;
            ServiceProvider = (IServiceProvider)Container;
#endif
        }


        public interface IService { }

        public class Service : IService { }

        public class TestGeneric<T> { }
    }
}
