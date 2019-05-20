using BenchmarkDotNet.Attributes;
using Unity;

namespace Performance.Tests
{
    public class PreResolved : BasicBase
    {
        const string name = nameof(PreResolved);

        [IterationSetup]
        public override void SetupContainer()
        {
            _container = new UnityContainer().AddExtension(new ForceActivation());

            base.SetupContainer();

            for (var i = 0; i < 3; i++)
            {
                base.UnityContainer();
                base.LegacyFactory();
                base.Factory();
                base.Instance();
                base.Unregistered();
                base.Transient();
                base.Mapping();
                base.MappingToSingleton();
                base.GenericInterface();
                base.Array();
                base.Enumerable();
            }
        }

        [Benchmark(Description = "(" + name + ")     IUnityContainer", OperationsPerInvoke = 20)]
        public override object UnityContainer() => base.UnityContainer();

        [Benchmark(Description = " IUnityContainerAsync", OperationsPerInvoke = 20)]
        public override object UnityContainerAsync() => base.UnityContainerAsync();

        [Benchmark(Description = "Factory (c,t,n)=>new Foo()", OperationsPerInvoke = 20)]
        public override object Factory() => base.Factory();

        [Benchmark(Description = "Factory        (with name)", OperationsPerInvoke = 20)]
        public override object LegacyFactory() => base.LegacyFactory();

        [Benchmark(Description = "Instance", OperationsPerInvoke = 20)]
        public override object Instance() => base.Instance();

        [Benchmark(Description = "Unregistered type", OperationsPerInvoke = 20)]
        public override object Unregistered() => base.Unregistered();

        [Benchmark(Description = "Registered type with dependencies", OperationsPerInvoke = 20)]
        public override object Transient() => base.Transient();

        [Benchmark(Description = "Registered interface to type mapping", OperationsPerInvoke = 20)]
        public override object Mapping() => base.Mapping();

        [Benchmark(Description = "Registered generic type mapping", OperationsPerInvoke = 20)]
        public override object GenericInterface() => base.GenericInterface();

        [Benchmark(Description = "Mapping to Singleton", OperationsPerInvoke = 20)]
        public override object MappingToSingleton() => base.MappingToSingleton();

        [Benchmark(Description = "Array", OperationsPerInvoke = 20)]
        public override object Array() => base.Array();

        [Benchmark(Description = "Enumerable", OperationsPerInvoke = 20)]
        public override object Enumerable() => base.Enumerable();
    }
}
