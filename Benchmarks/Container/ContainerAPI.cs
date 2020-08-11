using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
#if NET462
using Microsoft.Practices.Unity;
#else
using Unity.Injection;
using Unity.Lifetime;
using Unity;
#endif

namespace Unity.Benchmarks
{
    public class ContainerAPI
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
        [BenchmarkCategory("check", "true")]
        public virtual bool IsRegistered() 
            => Container.IsRegistered(typeof(IUnityContainer), null);


        [Benchmark(Description = "Container.IsRegistered(false)")]
        [BenchmarkCategory("check", "false")]
        public virtual bool IsNotRegistered() 
            => Container.IsRegistered(typeof(object), null);
    }
}
