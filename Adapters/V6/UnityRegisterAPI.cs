using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Unity.Benchmarks;

namespace Unity.v6
{
    [ShortRunJob(RuntimeMoniker.Net48)]
    public class UnityRegisterAPI : RegisterAPI
    {
    }
}
