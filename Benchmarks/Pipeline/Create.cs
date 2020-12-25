using BenchmarkDotNet.Attributes;
using Benchmarks;
#if UNITY_V4
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
        [Benchmark(Description = "Create Pipeline for Unknown" + BenchmarkBase.VERSION), BenchmarkCategory("Pipeline", "Create")]
        public object Pipeline_Create_Unknown() => Container.Resolve(typeof(object));


        [Benchmark(Description = "Create Pipeline for Registered" + BenchmarkBase.VERSION), BenchmarkCategory("Pipeline", "Create")]
        public object Pipeline_Create_Registered() => Container.Resolve(typeof(Service));
    }
}
