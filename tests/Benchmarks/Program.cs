using BenchmarkDotNet.Running;
using Unity.Container.Tests;

namespace Unity.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssemblies(new[] 
            {
                typeof(Program).Assembly,
                typeof(PatternTestMethodAttribute).Assembly,
            }).Run(args);
        }
    }
}
