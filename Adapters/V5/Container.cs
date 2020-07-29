using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Unity.Benchmarks.Container;

namespace Unity.v5
{
    [ShortRunJob(RuntimeMoniker.Net472)]
    public class Container : ContainerAPI
    {
    }
}
