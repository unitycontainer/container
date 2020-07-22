using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System.Reflection;

namespace Unity.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args, GetGlobalConfig());
        }

        static IConfig GetGlobalConfig()
            => DefaultConfig.Instance
                .AddJob(Job.Default.AsDefault())
                .WithOptions(ConfigOptions.DisableOptimizationsValidator);
    }
}
