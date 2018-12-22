using BenchmarkDotNet.Attributes;
using Runner.Setup;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace Runner.Tests
{
    [BenchmarkCategory("Basic")]
    [Config(typeof(BenchmarkConfiguration))]
    public class ColdStart
    {
        IUnityContainer _container;

        [IterationSetup]
        public virtual void SetupContainer()
        {
            _container = new UnityContainer();
            _container.RegisterType<Poco>();
            _container.RegisterType<IService, Service>();
            _container.RegisterType<IService, Service>("1");
            _container.RegisterType<IService, Service>("2");
        }

        [Benchmark]
        public object IUnityContainer() => _container.Resolve(typeof(IUnityContainer), null);

        [Benchmark]
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


        [Benchmark]
        public object Registrations() => _container.Registrations.ToArray();
    }

    public interface IService { }
    public class Service : IService { }

    public class Poco { }
}
