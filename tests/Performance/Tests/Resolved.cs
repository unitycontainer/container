using BenchmarkDotNet.Attributes;
using Runner.Setup;
using System.Collections.Generic;
using Unity;
using Unity.Extension;

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
            _container = new UnityContainer();
            _container.AddExtension(new Diagnostic())
                      .Configure<Diagnostic>()
                      .DisableCompile();
        }

        [Benchmark(Description = "Resolve<IUnityContainer>               ")]
        public object IUnityContainer() => _container.Resolve(typeof(IUnityContainer), null);

        [Benchmark(Description = "Resolve<object> (unregistered)")]
        public object Unregistered() => _container.Resolve(typeof(object), null);

        [Benchmark(Description = "Resolve<Poco>   (registered)")]
        public object Transient() => _container.Resolve(typeof(Poco), null);

        [Benchmark(Description = "Resolve<IService>   (registered)")]
        public object Mapping() => _container.Resolve(typeof(IFoo), null);

        [Benchmark(Description = "Resolve<IService>      (factory)")]
        public object Factory() => _container.Resolve(typeof(IFoo), "2");

        [Benchmark(Description = "Resolve<IService[]>   (registered)")]
        public object Array() => _container.Resolve(typeof(IFoo[]), null);

        [Benchmark(Description = "Resolve<IEnumerable<IService>>   (registered)")]
        public object Enumerable() => _container.Resolve(typeof(IEnumerable<IFoo>), null);
    }
}
