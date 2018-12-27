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
        object _syncRoot = new object();

        [IterationSetup]
        public virtual void SetupContainer()
        {
            _container = new UnityContainer();
            _container.RegisterType<Poco>();
            _container.RegisterType<IService, Service>();
            _container.RegisterType<IService, Service>("1");
            _container.RegisterType<IService, Service>("2");
        }

        [Benchmark(Description = "Resolve<IUnityContainer>")]
        public object IUnityContainer() => _container.Resolve(typeof(IUnityContainer), null);

        [Benchmark(Description = "Resolve<object> (unregistered)")]
        public object Unregistered() => _container.Resolve(typeof(object), null);

        [Benchmark(Description = "Resolve<Poco> (registered)")]
        public object Transient() => _container.Resolve(typeof(Poco), null);

        [Benchmark(Description = "Resolve<IService> (registered)")]
        public object Mapping() => _container.Resolve(typeof(IService), null);

        [Benchmark(Description = "Resolve<IService[]> (registered)")]
        public object Array() => _container.Resolve(typeof(IService[]), null);

        [Benchmark(Description = "Resolve<IEnumerable<IService>> (registered)")]
        public object Enumerable() => _container.Resolve(typeof(IEnumerable<IService>), null);


        public interface IService { }
        public class Service : IService { }

        public class Poco { }
    }
}
