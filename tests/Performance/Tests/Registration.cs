using BenchmarkDotNet.Attributes;
using Runner.Setup;
using Unity;

namespace Runner.Tests
{
    [BenchmarkCategory("Registration")]
    [Config(typeof(BenchmarkConfiguration))]
    public class Registration
    {
        IUnityContainer _container;
        object _syncRoot = new object();

        [IterationSetup]
        public virtual void SetupContainer()
        {
            _container = new UnityContainer();
        }
        [Benchmark]
        public object Object() => _container.RegisterType(null, typeof(object), null, null);

        [Benchmark]
        public object Mapping() => _container.RegisterType(typeof(IFoo), typeof(Foo), null, null);

        [Benchmark]
        public object Instance() => _container.RegisterInstance(null, null, _syncRoot, null);
    }

    public interface IFoo { }
    public class Foo : IFoo { }
}
