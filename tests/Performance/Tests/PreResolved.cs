using BenchmarkDotNet.Attributes;
using Runner.Setup;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Unity.Builder;

namespace Runner.Tests
{
    [BenchmarkCategory("Basic")]
    [Config(typeof(BenchmarkConfiguration))]
    public class PreResolved
    {
        IUnityContainer _container;
        object _syncRoot = new object();

        [IterationSetup]
        public virtual void SetupContainer()
        {
            _container = new UnityContainer(ModeFlags.Activated);

            _container.RegisterType(typeof(IFoo<>), typeof(Foo<>));
            _container.RegisterType<Poco>();
            _container.RegisterType<IFoo, Foo>();
            _container.RegisterType<IFoo, Foo>("1");
            _container.RegisterFactory<IFoo>("2", c => new Foo());

            for (var i = 0; i < 3; i++)
            {
                _container.Resolve(typeof(object), null);
                _container.Resolve(typeof(Poco), null);
                _container.Resolve(typeof(IFoo), null);
                _container.Resolve(typeof(IFoo), "1");
                _container.Resolve(typeof(IFoo), "2");
                _container.Resolve(typeof(IFoo[]), null);
                _container.Resolve(typeof(IFoo<IFoo>), null);
                _container.Resolve(typeof(IEnumerable<IFoo>), null);
            }
        }

        [Benchmark(Description = "Resolve<IUnityContainer>            ")]
        public object UnityContainer() => _container.Resolve(typeof(IUnityContainer), null);

        [Benchmark(Description = "PreResolved<object> (optimized)")]
        public object Unregistered() => _container.Resolve(typeof(object), null);

        [Benchmark(Description = "PreResolved<Poco> (optimized)")]
        public object Transient() => _container.Resolve(typeof(Poco), null);

        [Benchmark(Description = "PreResolved<IService> (optimized)")]
        public object Mapping() => _container.Resolve(typeof(IFoo), null);

        [Benchmark(Description = "PreResolved<IFoo<IService>> (optimized)")]
        public object GenericInterface() => _container.Resolve(typeof(IFoo<IFoo>), null);

        [Benchmark(Description = "Resolved<IService>   (factory)")]
        public object LegacyFactory() => _container.Resolve(typeof(IFoo), "2");

        [Benchmark(Description = "PreResolved<IService[]> (optimized)")]
        public object Array() => _container.Resolve(typeof(IFoo[]), null);

        [Benchmark(Description = "PreResolved<IEnumerable<IService>> (optimized)")]
        public object Enumerable() => (_container.Resolve(typeof(IEnumerable<IFoo>), null) as IEnumerable<IFoo>)?.ToArray();
    }
}
