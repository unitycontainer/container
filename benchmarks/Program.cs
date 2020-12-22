using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System.Reflection;
using Benchmarks;

namespace Unity.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssemblies(new[] 
            { 
                typeof(BenchmarksBase).Assembly,
                Assembly.GetExecutingAssembly() 
            }).Run(args, GetGlobalConfig());
        }

        static IConfig GetGlobalConfig()
            => DefaultConfig.Instance
                .AddJob(Job.Default.AsDefault())
                .WithOptions(ConfigOptions.DisableOptimizationsValidator);
    }
}
