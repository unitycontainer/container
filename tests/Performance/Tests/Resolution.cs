using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Runner.Tests;
using Unity;

namespace Performance.Tests
{
    [BenchmarkCategory("Resolution")]
    [MemoryDiagnoser]
    public class Resolution
    {
        private IUnityContainer _container;

        [GlobalSetup]
        public void SetupContainer()
        {
            _container = new UnityContainer();
            _container.RegisterType<Poco>();
            _container.RegisterType<IService, Service>();
            _container.RegisterType<IService, Service>("1");
            _container.RegisterType<IService, Service>("2");
        }

        [Benchmark(Baseline = true)]
        public object Container() => _container.Resolve(typeof(IUnityContainer), null);

        [Benchmark]
        public object Unregistered() => _container.Resolve(typeof(object), null);

        [Benchmark]
        public object Transient() => _container.Resolve(typeof(Poco), null);

        [Benchmark]
        public object Mapping() => _container.Resolve(typeof(IService), null);

        [Benchmark]
        public object Array() => _container.Resolve(typeof(IService[]), null);

        [Benchmark]
        public object Enumerable() => _container.Resolve(typeof(IEnumerable<IService>), null);
    }
}
