using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Unity.v5
{
    [SimpleJob(RuntimeMoniker.Net472)]
    public class Create : Benchmarks.Create
    {
    }
}
