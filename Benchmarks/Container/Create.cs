using BenchmarkDotNet.Attributes;
using Benchmarks;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity.Injection;
using Unity.Lifetime;
using Unity;
#endif

namespace Unity.Benchmarks
{
    public partial class Create
    {
        private static IUnityContainer Container;


        [GlobalSetup]
        public static void InitializeClass()
        {
            Container = new UnityContainer();
        }


        [Benchmark(Description = "new UnityContainer()" + BenchmarkBase.VERSION), BenchmarkCategory("new", "UnityContainer")]
        public object New_UnityContainer() => new UnityContainer();



        [Benchmark(Description = "CreateChildContainer()" + BenchmarkBase.VERSION), BenchmarkCategory("new", "ChildContainer")]
        public object CreateChildContainer() => Container.CreateChildContainer();
    }
}
