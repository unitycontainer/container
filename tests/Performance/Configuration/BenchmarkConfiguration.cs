using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Validators;
using System.Linq;

namespace Runner.Setup
{
    public class BenchmarkConfiguration : ManualConfig
    {
        public BenchmarkConfiguration()
        {
            Add(JitOptimizationsValidator.DontFailOnError); // ALLOW NON-OPTIMIZED DLLS

            Add(Job.Default
                   .WithUnrollFactor(1)
                   .WithInvocationCount(50000));
        }
    }
}
