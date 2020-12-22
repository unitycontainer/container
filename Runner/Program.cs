using BenchmarkDotNet.Running;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Unity.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkSwitcher.FromAssemblies(new[]
            //{
            //    typeof(Unity.v4.BenchmarksBase).Assembly,
            //    typeof(Unity.v5.BenchmarksBase).Assembly,
            //    typeof(Unity.v6.BenchmarksBase).Assembly,
            //}).Run(args);
        }
    }
}
