using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;


namespace Unity.V6
{
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    public class Create : Benchmarks.Create
    {
    }
}
