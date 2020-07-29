using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Unity.Benchmarks.Container;

namespace Unity.v6
{
    [ShortRunJob(RuntimeMoniker.Net48)]
    public class Container : ContainerAPI
    {
    }
}
