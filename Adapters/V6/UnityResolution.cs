using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Unity.Benchmarks;

namespace Unity.v6
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    public class UnityResolution : ResolutionBenchmarks
    {
    }
}
