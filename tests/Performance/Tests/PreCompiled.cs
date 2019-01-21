using BenchmarkDotNet.Attributes;
using Runner.Setup;
using System.Collections.Generic;
using Unity;
using Unity.Builder;

namespace Runner.Tests
{
    [BenchmarkCategory("Basic")]
    [Config(typeof(BenchmarkConfiguration))]
    public class PreCompiled
    {
        IUnityContainer _container;
        object _syncRoot = new object();

        [IterationSetup]
        public virtual void SetupContainer()
        {
            _container = new UnityContainer(Unity.UnityContainer.BuildStrategy.Compiled);

            _container.RegisterType<Poco>();
            _container.RegisterType<IFoo, Foo>();
            _container.RegisterType<IFoo, Foo>("1");
            _container.RegisterFactory<IFoo>("2", c => new Foo());

            for (var i = 0; i < 3; i++)
            {
                _container.Resolve<object>();
                _container.Resolve<Poco>();
                _container.Resolve<IFoo>();
                _container.Resolve<IFoo>("1");
                _container.Resolve<IFoo>("2");
            }
        }

        [Benchmark(Description = "Resolve<IUnityContainer>            ")]
        public object UnityContainer() => _container.Resolve(typeof(IUnityContainer), null);

        [Benchmark(Description = "PreCompiled<object> (pre-built)")]
        public object Unregistered() => _container.Resolve(typeof(object), null);

        [Benchmark(Description = "PreCompiled<Poco> (pre-built)")]
        public object Transient() => _container.Resolve(typeof(Poco), null);

        [Benchmark(Description = "PreCompiled<IService> (pre-built)")]
        public object Mapping() => _container.Resolve(typeof(IFoo), null);

        [Benchmark(Description = "Compiled<IService>      (legacy)")]
        public object LegacyFactory() => _container.Resolve(typeof(IFoo), "2");

        [Benchmark(Description = "PreCompiled<IService[]> (pre-built)")]
        public object Array() => _container.Resolve(typeof(IFoo[]), null);

        [Benchmark(Description = "PreCompiled<IEnumerable<IService>> (pre-built)")]
        public object Enumerable() => _container.Resolve(typeof(IEnumerable<IFoo>), null);
    }
}
