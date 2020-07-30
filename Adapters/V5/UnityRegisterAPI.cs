using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Unity.Benchmarks;

namespace Unity.v5
{
    [ShortRunJob(RuntimeMoniker.Net472)]
    public class UnityRegisterAPI : RegisterAPI
    {
    }
}
