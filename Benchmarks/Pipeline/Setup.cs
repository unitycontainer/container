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
            Container = new UnityContainer()
                .AddExtension(new PipelineSpyExtension())
                .RegisterType<Service>();
        }
    }


    #region Test Data

    public class Service
    { 
    
    }
    
    #endregion
}
