using BenchmarkDotNet.Attributes;
using Runner.Setup;
using System.Collections.Generic;
using Unity;

namespace Runner.Tests
{
    [BenchmarkCategory("Basic")]
    [Config(typeof(BenchmarkConfiguration))]
    public class Resolved
    {
        IUnityContainer _container;
        object _syncRoot = new object();

        [IterationSetup]
        public virtual void SetupContainer()
        {
            _container = new UnityContainer(UnityContainer.BuildStrategy.Resolved);

            _container.RegisterType<Poco>();
            _container.RegisterType<IFoo, Foo>();
            _container.RegisterType<IFoo, Foo>("1");
            _container.RegisterType<IFoo>("2", Invoke.Factory(c => new Foo()));
        }

        [Benchmark(Description = "Resolve<IUnityContainer>               ")]
        public object IUnityContainer() => _container.Resolve(typeof(IUnityContainer), null);

        [Benchmark(Description = "Resolved<object> (unregistered)")]
        public object Unregistered() => _container.Resolve(typeof(object), null);

        [Benchmark(Description = "Resolved<Poco>   (registered)")]
        public object Transient() => _container.Resolve(typeof(Poco), null);

        [Benchmark(Description = "Resolved<IService>   (registered)")]
        public object Mapping() => _container.Resolve(typeof(IFoo), null);

        [Benchmark(Description = "Resolved<IService>      (factory)")]
        public object Factory() => _container.Resolve(typeof(IFoo), "2");

        [Benchmark(Description = "Resolved<IService[]>   (registered)")]
        public object Array() => _container.Resolve(typeof(IFoo[]), null);

        [Benchmark(Description = "Resolved<IEnumerable<IService>>   (registered)")]
        public object Enumerable() => _container.Resolve(typeof(IEnumerable<IFoo>), null);
    }
}
