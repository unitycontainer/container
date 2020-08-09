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

        [Benchmark(Description = "Container.Resolve<IUnityContainer>(_)", Baseline = true)]
        [BenchmarkCategory("resolve", "interface")]
        public object Resolve_IUnityContainer()
            => Container.Resolve(typeof(IUnityContainer), null);

        [Benchmark(Description = "Container.Resolve<IServiceProvider>()")]
        [BenchmarkCategory("resolve", "interface")]
#if NET462 || NET472
        public object Resolve_IServiceProvider()
            => throw new NotImplementedException();
#else
        public object Resolve_IServiceProvider()
            => Container.Resolve(typeof(IServiceProvider), null);
#endif
                                                    
        [Benchmark(Description = "Container.R*Async<IUnityCo...Async>()")]
        [BenchmarkCategory("resolve", "async", "interface")]
#if NET462 || NET472
        public object Resolve_IUnityContainerAsync()
            => throw new NotImplementedException();
#else
        public object Resolve_IUnityContainerAsync()
            => ContainerAsync.ResolveAsync(typeof(IUnityContainerAsync), (string)null)
                             .GetAwaiter()
                             .GetResult();
#endif
    }
}
