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
            _container.RegisterType<IService>("2", Invoke.Factory(c => new Service()));

            _container.Resolve<object>();
            _container.Resolve<Poco>();
            _container.Resolve<IService>();
            _container.Resolve<IService>("1");
            _container.Resolve<IService>("2");
        }

        [Benchmark(Description = "Resolve<IUnityContainer>            ")]
        public object IUnityContainer() => _container.Resolve(typeof(IUnityContainer), null);

        [Benchmark(Description = "Resolve<object> (pre-built)")]
        public object Unregistered() => _container.Resolve(typeof(object), null);

        [Benchmark(Description = "Resolve<Poco> (pre-built)")]
        public object Transient() => _container.Resolve(typeof(Poco), null);

        [Benchmark(Description = "Resolve<IService> (pre-built)")]
        public object Mapping() => _container.Resolve(typeof(IService), null);

        [Benchmark(Description = "Resolve<IService>   (factory)")]
        public object Factory() => _container.Resolve(typeof(IService), "2");

        [Benchmark(Description = "Resolve<IService[]> (pre-built)")]
        public object Array() => _container.Resolve(typeof(IService[]), null);

        [Benchmark(Description = "Resolve<IEnumerable<IService>> (pre-built)")]
        public object Enumerable() => _container.Resolve(typeof(IEnumerable<IService>), null);


        public interface IService { }
        public class Service : IService { }

        public class Poco { }
    }
}
