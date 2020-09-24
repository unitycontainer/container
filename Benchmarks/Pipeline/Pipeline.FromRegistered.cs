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
        public object FromRegistered_Resolving_Transient()
            => Container.Resolve(typeof(Service), EveryTime);

        [Benchmark]                            
        public object FromRegistered_Resolving_Balanced_()
            => Container.Resolve(typeof(Service), OnceInAWhile);

        [Benchmark]
        public object FromRegistered_Resolving_Singleton()
            => Container.Resolve(typeof(Service), OnceInLifetime);



        [Benchmark]
        public object FromRegistered_Pipeline_Singleton()
            => Container.Resolve(typeof(OtherService), OnceInLifetime);

        [Benchmark]
        public object FromRegistered_Pipeline_Balanced_()
            => Container.Resolve(typeof(OtherService), OnceInAWhile);

        [Benchmark]
        public object FromRegistered_Pipeline_Transient()
            => Container.Resolve(typeof(OtherService), EveryTime);



        [Benchmark]
        public object FromRegistered_Complete_Transient()
            => Container.Resolve(typeof(CompleteService), EveryTime);

        [Benchmark]
        public object FromRegistered_Complete_Balanced_()
            => Container.Resolve(typeof(CompleteService), OnceInAWhile);

        [Benchmark]
        public object FromRegistered_Complete_Singleton()
            => Container.Resolve(typeof(CompleteService), OnceInLifetime);
    }
}
