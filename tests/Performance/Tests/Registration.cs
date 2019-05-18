using System;
using BenchmarkDotNet.Attributes;
using Runner.Setup;
using System.Linq;
using Unity;

namespace Performance.Tests
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

        [Benchmark(Description = "Register (No Mapping)")]
        public object Register() => _container.RegisterType((Type)null, typeof(object), null, null);

        [Benchmark(Description = "Register Mapping")]
        public object RegisterMapping() => _container.RegisterType(typeof(IFoo), typeof(Foo), null, null);

        [Benchmark(Description = "Register Instance")]
        public object RegisterInstance() => _container.RegisterInstance(null, null, _syncRoot, null);

        [Benchmark(Description = "Registrations.ToArray()")]
        public object Registrations() => _container.Registrations.ToArray();

        [Benchmark(Description = "IsRegistered (True)")]
        public object IsRegistered() => _container.IsRegistered(typeof(IUnityContainer));

        [Benchmark(Description = "IsRegistered (False)")]
        public object IsRegisteredFalse() => _container.IsRegistered(typeof(IFoo));
    }
}
