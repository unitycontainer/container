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
    public partial class PipelineBenchmarks
    {
        [Benchmark]
        public object FromRegistered_Complete_Transient()
            => Container.Resolve(typeof(Service), EveryTime);

        [Benchmark]
        public object FromRegistered_Complete_Balanced_()
            => Container.Resolve(typeof(Service), OnceInAWhile);

        [Benchmark]
        public object FromRegistered_Complete_Singleton()
            => Container.Resolve(typeof(Service), OnceInLifetime);
    }
}
