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

        [Benchmark(Description = "IUnityContainer<object>()")]
        [BenchmarkCategory("resolve", "object", "IUnityContainer")]
        public object Resolve_Object_IUnityContainer()
            => Container.Resolve(typeof(object), null);

#if !NET462 && !NET472

        [Benchmark(Description = "IServiceProvider<object>()")]
        [BenchmarkCategory("resolve", "object", "IServiceProvider")]
        public object Resolve_Object_IServiceProvider()
            => ServiceProvider.GetService(typeof(object));

        [Benchmark(Description = "IUnityContainerAsync<object>()")]
        [BenchmarkCategory("resolve", "object", "IUnityContainerAsync")]
        public object Resolve_Object_IUnityContainerAsync()
            => ContainerAsync.ResolveAsync(typeof(object), null)
                             .GetAwaiter()
                             .GetResult();
#endif
    }
}
