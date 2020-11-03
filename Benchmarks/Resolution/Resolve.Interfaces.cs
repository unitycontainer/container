using BenchmarkDotNet.Attributes;
using System;
using System.Runtime.CompilerServices;
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
        [Benchmark(Description = "IUnityContainer<IUnityContainer >( )")]
        public object Resolve_IUnityContainer_IUnityContainer()
            => Container.Resolve(typeof(IUnityContainer), null);
             
        [Benchmark(Description = "IUnityContainer<IServiceProvider>( )")]
#if NET462 || NET472
        public object Resolve_IServiceProvider()
            => throw new NotImplementedException();
#else
        public object Resolve_IUnityContainer_IServiceProvider()
            => Container.Resolve(typeof(IServiceProvider), null);
#endif
                                                    
        [Benchmark(Description = "IUnityContainerAsync<IUnityContainer>( )")]
#if NET462 || NET472
        public object Resolve_IUnityContainerAsync()
            => throw new NotImplementedException();
#else
        public object Resolve_AsyncIUnityContainer()
            => ContainerAsync.ResolveAsync(typeof(IUnityContainerAsync), (string)null)
                             .GetAwaiter()
                             .GetResult();
#endif


#if !NET46 && !NET47 && !NET462 && !NET472
        [Benchmark(Description = "IServiceProvider<IUnityContainer >( )")]
        public object Resolve_IServiceProvider_IUnityContainer()
            => Unsafe.As<IServiceProvider>(Container).GetService(typeof(IUnityContainer));

        [Benchmark(Description = "IServiceProvider<IServiceProvider>( )")]
        public object Resolve_IServiceProvider_IServiceProvider()
            => Unsafe.As<IServiceProvider>(Container).GetService(typeof(IServiceProvider));
#endif
    }
}
