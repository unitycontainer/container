﻿using BenchmarkDotNet.Attributes;
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

        [Benchmark(Description = "Container.Resolve<IUnityContainer >( )")]
        [BenchmarkCategory("resolve", "registered", "interface", "IUnityContainer")]
        public object Resolve_IUnityContainer()
            => Container.Resolve(typeof(IUnityContainer), null);

             
        [Benchmark(Description = "Container.Resolve<IServiceProvider>( )")]
        [BenchmarkCategory("resolve", "registered", "interface", "IServiceProvider")]
#if NET462 || NET472
        public object Resolve_IServiceProvider()
            => throw new NotImplementedException();
#else
        public object Resolve_IServiceProvider()
            => Container.Resolve(typeof(IServiceProvider), null);
#endif
                                                    
        [Benchmark(Description = "Container.R*Async<IUnityCont*Async>( )")]
        [BenchmarkCategory("resolve", "registered", "interface", "IUnityContainerAsync")]
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
