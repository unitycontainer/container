using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Unity.Benchmarks;

namespace Unity.v5
{
    [SimpleJob(RuntimeMoniker.Net472)]
    public class UnityResolution_v5 : ResolutionBenchmarks
    {
    }
}
