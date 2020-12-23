using BenchmarkDotNet.Running;
using Unity.v4;
using Unity.v5;
using Unity.v6;

namespace Unity.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssemblies(new[]
            {
                typeof(UnityAdapterV4).Assembly,
                typeof(UnityAdapterV5).Assembly,
                typeof(UnityAdapterV6).Assembly,
            }).Run(args);
        }
    }
}
