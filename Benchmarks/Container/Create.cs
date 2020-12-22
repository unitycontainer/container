using BenchmarkDotNet.Attributes;
using System;
using BenchmarkDotNet.Jobs;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity.Injection;
using Unity.Lifetime;
using Unity;
#endif

namespace Unity.Benchmarks
{
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    [SimpleJob(RuntimeMoniker.Net48)]
    [SimpleJob(RuntimeMoniker.Net472)]
    [SimpleJob(RuntimeMoniker.Net462)]
    public partial class Create
    {
#if   UNITY_V4
        [Benchmark(Description = "new UnityContainer(4)"), BenchmarkCategory("new", "UnityContainer")]
#elif UNITY_V5
        [Benchmark(Description = "new UnityContainer(5)"), BenchmarkCategory("new", "UnityContainer")]
#else
        [Benchmark(Description = "new UnityContainer(6)"), BenchmarkCategory("new", "UnityContainer")]
#endif

        public object New_UnityContainer() => new UnityContainer();
    }
}
