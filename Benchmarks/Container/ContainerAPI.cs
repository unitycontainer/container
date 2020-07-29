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
        protected IUnityContainer Container;
        protected Consumer Consumer = new Consumer();

        [GlobalSetup]
        public virtual void GlobalSetup() 
            => Container = new UnityContainer();


        [Benchmark(Description = "new UnityContainer()")]
        [BenchmarkCategory("new")]
        public object NewUnityContainer() 
            => new UnityContainer();


        [Benchmark(Description = "CreateChildContainer()")]
        [BenchmarkCategory("child")]
        public object CreateChildContainer() 
            => Container.CreateChildContainer();


        [Benchmark(Description = "Container.Registrations")]
        [BenchmarkCategory("registrations")]
        public object Registrations() 
            => Container.Registrations;


        [Benchmark(Description = "Registrations.ToArray()")]
        [BenchmarkCategory("registrations", "ToArray")]
        public void RegistrationsToArray() 
            => Container.Registrations.Consume(Consumer);


        [Benchmark(Description = "Container.IsRegistered(true)")]
        [BenchmarkCategory("registration", "true")]
        public virtual bool IsRegistered() 
            => Container.IsRegistered(typeof(IUnityContainer), null);


        [Benchmark(Description = "Container.IsRegistered(false)")]
        [BenchmarkCategory("registration", "false")]
        public virtual bool IsNotRegistered() 
            => Container.IsRegistered(typeof(object), "name");
    }
}
