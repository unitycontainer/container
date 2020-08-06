using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Unity.Benchmarks;

namespace Unity.v4
{
    [ShortRunJob(RuntimeMoniker.Net462)]
    public class UnityResolution : ResolutionBenchmarks
    {
    }
}
