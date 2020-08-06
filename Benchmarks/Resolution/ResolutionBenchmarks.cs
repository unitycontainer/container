using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System.ComponentModel;
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
    public class ResolutionBenchmarks
    {
        protected IUnityContainer Container;

        [GlobalSetup]
        public virtual void GlobalSetup()
        {
            Container = new UnityContainer()
                .RegisterType(typeof(List<>), new InjectionConstructor())
                .RegisterType(typeof(Service))
                .CreateChildContainer();
        }

        [Benchmark(Description = "Resolve<IUnityContainer>()")]
        [BenchmarkCategory("resolve")]
        public object Resolve_IUnityContainer()
            => Container.Resolve(typeof(IUnityContainer), null);

        [Benchmark(Description = "Resolve<IUnityContainerAsync>()")]
        [BenchmarkCategory("resolve")]
        public object Resolve_IUnityContainerAsync()
#if NET48
            => Container.Resolve(typeof(IUnityContainerAsync), null);
#else
            => throw new NotImplementedException();
#endif

        //[Benchmark(Description = "Resolve<object>()")]
        //[BenchmarkCategory("resolve")]
        //public object Resolve_Object()
        //    => Container.Resolve(typeof(object), null);

        [Benchmark(Description = "Resolve<from-factory>()")]
        [BenchmarkCategory("resolve")]
        public object Resolve_Generic()
            => Container.Resolve(typeof(List<int>), null);

        public interface IService { }

        public class Service : IService { }

        public class TestGeneric<T> { }
    }
}
