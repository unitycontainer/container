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

        [Benchmark(Description = "IUnityContainer.Resolve<Service>()")]
        [BenchmarkCategory("resolve", "registered", "IUnityContainer")]
        public object Resolve_Registered_Type()
            => Container.Resolve(typeof(Service), null);

        [Benchmark(Description = "IUnityContainerAsync.ReAsync<Service>()")]
        [BenchmarkCategory("resolve", "registered", "IUnityContainerAsync")]
#if NET462 || NET472
        public object Resolve_Object_IUnityContainerAsync()
            => throw new NotImplementedException();
#else
        public object Resolve_Registered_Type_Async()
            => ContainerAsync.ResolveAsync(typeof(Service), (string)null)
                             .GetAwaiter()
                             .GetResult();
#endif
    }
}
