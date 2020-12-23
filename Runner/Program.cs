using BenchmarkDotNet.Running;
#if STANDALONE
using Unity.v4;
using Unity.v5;
using Unity.v6;
#else
using Benchmarks;
#endif

namespace Unity.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssemblies(new[]
            {
#if STANDALONE
                typeof(UnityAdapterV4).Assembly,
                typeof(UnityAdapterV5).Assembly,
                typeof(UnityAdapterV6).Assembly,
#else
                typeof(BenchmarkBase).Assembly
#endif
            }).Run(args);
        }
    }
}
