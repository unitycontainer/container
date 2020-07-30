using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Unity.Benchmarks
{
    [ShortRunJob(RuntimeMoniker.NetCoreApp50)]
    public class UnityContainerAPI : ContainerAPI
    {
    }
}
