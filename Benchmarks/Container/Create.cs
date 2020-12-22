using BenchmarkDotNet.Attributes;
using System;
using BenchmarkDotNet.Jobs;
#if UNITY_V4
using Microsoft.Practices.Unity;

namespace Unity.v4
{
    [SimpleJob(RuntimeMoniker.Net462)]

#elif UNITY_V5

namespace Unity.v5
{
    [SimpleJob(RuntimeMoniker.Net472)]

#else
using Unity.Injection;
using Unity.Lifetime;
using Unity;

namespace Unity.v6
{
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    [SimpleJob(RuntimeMoniker.Net48)]
#endif
    public partial class Create
    {
        private static IUnityContainer Container;

        [GlobalSetup]
        public static void InitializeClass()
        {
            Container = new UnityContainer();
        }

        //[IterationSetup]
        //public void IterationSetup()
        //{
        //}

        //[IterationCleanup]
        //public void IterationCleanup()
        //{
        //}


        [Benchmark(Description = "new UnityContainer()"), BenchmarkCategory("new", "UnityContainer")]
        public object New_UnityContainer() => new UnityContainer();



        [Benchmark(Description = "CreateChildContainer()"), BenchmarkCategory("new", "ChildContainer")]
        public object CreateChildContainer() => Container.CreateChildContainer();
    }
}
