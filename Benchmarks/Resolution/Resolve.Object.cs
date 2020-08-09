using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System.ComponentModel;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
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

        [Benchmark(Description = "IUnityContainer.Resolve<object>()")]
        [BenchmarkCategory("resolve", "object")]
        public object Resolve_Object_IUnityContainer()
            => Container.Resolve(typeof(object), null);

        [Benchmark(Description = "IServiceProvider.Resolve<object>()")]
        [BenchmarkCategory("resolve", "object")]
#if NET462 || NET472
        public object Resolve_Object_IServiceProvider()
            => throw new NotImplementedException();
#else
        public object Resolve_Object_IServiceProvider()
            => ContainerAsync.Resolve(typeof(object), null);
#endif
                                                       
        [Benchmark(Description = "IUnityContainerAsync.ReAsync<object>()")]
        [BenchmarkCategory("resolve", "async", "object")]
#if NET462 || NET472
        public object Resolve_Object_IUnityContainerAsync()
            => throw new NotImplementedException();
#else
        public object Resolve_Object_IUnityContainerAsync()
            => ContainerAsync.ResolveAsync(typeof(object), (string)null)
                             .GetAwaiter()
                             .GetResult();
#endif
    }
}
