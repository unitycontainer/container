using BenchmarkDotNet.Running;
using Unity.Benchmarks.Storage;

namespace Unity.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromTypes(new[] 
            { 
                typeof(ArrayBenchmarks) 

            }).Run(args);
        }
    }
}
