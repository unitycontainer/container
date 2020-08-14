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
        [Benchmark(Description = "IUnityContainer.Resolve<Generic>()")]
        [BenchmarkCategory("resolve", "generic", "IUnityContainer")]
        public object Resolve_Generic_IUnityContainer()
            => Container.Resolve(typeof(TestGeneric<object>), null);

        [Benchmark(Description = "IServiceProvider.Resolve<Generic>()")]
        [BenchmarkCategory("resolve", "generic", "IServiceProvider")]
#if NET462 || NET472
        public object Resolve_Generic_IServiceProvider()
            => throw new NotImplementedException();
#else
        public object Resolve_Generic_IServiceProvider()
            => ContainerAsync.Resolve(typeof(TestGeneric<object>), null);
#endif
                                                       
        [Benchmark(Description = "IUnityContainerAsync.ReAsync<Generic>()")]
        [BenchmarkCategory("resolve", "generic", "IUnityContainerAsync")]
#if NET462 || NET472
        public object Resolve_Generic_IUnityContainerAsync()
            => throw new NotImplementedException();
#else
        public object Resolve_Generic_IUnityContainerAsync()
            => ContainerAsync.ResolveAsync(typeof(TestGeneric<object>), (string)null)
                             .GetAwaiter()
                             .GetResult();
#endif
    }
}
