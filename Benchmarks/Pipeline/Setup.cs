using BenchmarkDotNet.Attributes;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity.Injection;
using Unity.Lifetime;
using Unity;
#endif

namespace Unity.Benchmarks
{
    public partial class PipelineBenchmarks
    {
        private static IUnityContainer Container;


        [GlobalSetup]
        public static void InitializeClass()
        {
            var unity = new UnityContainer()
                .AddExtension(new PipelineSpyExtension());
            
            Container = unity;
        }
    }
}
