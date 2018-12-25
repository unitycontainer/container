using BenchmarkDotNet.Attributes;
using Runner.Setup;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace Runner.Tests
{
    [BenchmarkCategory("Basic")]
    [Config(typeof(BenchmarkConfiguration))]
    public class PreBuilt
    {
        IUnityContainer _container;
        object _syncRoot = new object();

        [IterationSetup]
        public virtual void SetupContainer()
        {
            _container = new UnityContainer();
            _container.RegisterType<Poco>();
            _container.RegisterType<IService, Service>();
            _container.RegisterType<IService, Service>("1");
            _container.RegisterType<IService, Service>("2");

            _container.Resolve<object>();
            _container.Resolve<Poco>();
            _container.Resolve<IService>();
            _container.Resolve<IService>("1");
            _container.Resolve<IService>("2");
        }

        [Benchmark]
        public object IUnityContainer() => _container.Resolve(typeof(IUnityContainer), null);

        [Benchmark]
        public object PreUnregistered() => _container.Resolve(typeof(object), null);

        [Benchmark]
        public object PreTransient() => _container.Resolve(typeof(Poco), null);

        [Benchmark]
        public object PreMapping() => _container.Resolve(typeof(IService), null);

        [Benchmark]
        public object PreArray() => _container.Resolve(typeof(IService[]), null);

        [Benchmark]
        public object PreEnumerable() => _container.Resolve(typeof(IEnumerable<IService>), null);

        [Benchmark]
        public object Registrations() => _container.Registrations.ToArray();

        public interface IService { }
        public class Service : IService { }

        public class Poco { }
    }
}
