using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Unity.Benchmarks.Container;

namespace Unity.v4
{
    [ShortRunJob(RuntimeMoniker.Net462)]
    public class Container : ContainerAPI
    {
    }
}
