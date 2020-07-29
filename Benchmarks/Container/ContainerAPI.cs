using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
#if NET462
using Microsoft.Practices.Unity;
#else
using Unity.Injection;
using Unity.Lifetime;
using Unity;
#endif

namespace Unity.Benchmarks
{
    public partial class ContainerAPI
    {
        protected IUnityContainer Container = new UnityContainer();
        protected Consumer Consumer = new Consumer();

        [Benchmark(Description = "new UnityContainer()")]
        [BenchmarkCategory("new")]
        public object NewUnityContainer()
        {
            return new UnityContainer();
        }

        [Benchmark(Description = "CreateChildContainer()")]
        [BenchmarkCategory("child")]
        public object CreateChildContainer()
        {
            return Container.CreateChildContainer(); 
        }

        [Benchmark(Description = "Container.Registrations")]
        [BenchmarkCategory("registrations")]
        public object Registrations()
        {
            return Container.Registrations;
        }

        [Benchmark(Description = "Registrations.ToArray()")]
        [BenchmarkCategory("registrations", "ToArray")]
        public void RegistrationsToArray()
        {
            Container.Registrations.Consume(Consumer);
        }
    }
}
