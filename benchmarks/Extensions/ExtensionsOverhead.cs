using BenchmarkDotNet.Attributes;
using System;
using Unity;
using Unity.Lifetime;
using Unity.Resolution;

namespace Benchmarks.Extensions
{
    [MarkdownExporterAttribute.GitHub]
    public class ExtensionsOverhead
    {
        private static IFoo Instance = new Foo();
        private static LifetimeManager Manager = new ContainerControlledLifetimeManager();
        protected static Func<IUnityContainer, Type, string?, ResolverOverride[], object> Factory = (c, t, n, o) => c;

        [Benchmark(Description = "new RegistrationDescriptor()", OperationsPerInvoke = 3)]
        public virtual object RegistrationDescriptor()
        {
            var array = new RegistrationDescriptor[] 
            {
                new RegistrationDescriptor(typeof(Foo), null, (ITypeLifetimeManager)Manager, typeof(IFoo)),
                new RegistrationDescriptor(Instance, null, (IInstanceLifetimeManager)Manager, typeof(IFoo)),
                new RegistrationDescriptor(Factory, null, (IFactoryLifetimeManager)Manager, typeof(IFoo)),
            };

            return array;
        }


        [Benchmark(Description = "RegisterXXX()", OperationsPerInvoke = 3)]
        public virtual object RegisterItem()
        {
            var array = new RegistrationDescriptor[]
            {
                new RegistrationDescriptor(typeof(Foo), null, (ITypeLifetimeManager)    Manager, typeof(IFoo)),
                new RegistrationDescriptor(   Instance, null, (IInstanceLifetimeManager)Manager, typeof(IFoo)),
                new RegistrationDescriptor(Factory, null, (IFactoryLifetimeManager) Manager, typeof(IFoo)),
            };

            return array;
        }

        public interface IFoo { }
        public class Foo : IFoo { }
    }
}
