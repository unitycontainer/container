using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Unity.v4
{
    [SimpleJob(RuntimeMoniker.Net462)]
    public class Create : Benchmarks.Create
    {
    }
}
