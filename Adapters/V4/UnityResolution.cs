using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Unity.Benchmarks;

namespace Unity.v4
{
    [SimpleJob(RuntimeMoniker.Net462)]
    public class UnityResolution : ResolutionBenchmarks
    {
    }
}
