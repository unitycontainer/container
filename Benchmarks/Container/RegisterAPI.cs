using BenchmarkDotNet.Attributes;
using System;
#if NET462
using Microsoft.Practices.Unity;
#else
using Unity.Lifetime;
#endif

namespace Unity.Benchmarks
{
    public class RegisterAPI
    {
        protected static LifetimeManager Manager = new ContainerControlledLifetimeManager();
        protected Func<IUnityContainer, Type, string, object> Factory = (c, t, n) => c;
        protected const string Name = "name";
        protected IUnityContainer Container;
        protected object Instance1 = new object();
        protected object Instance2 = new object();
        protected object Instance3 = "object";

        [IterationSetup]
        public void IterationSetup()
        { 
            Container = new UnityContainer(); 
        }

        [Benchmark(Description = "RegisterType()", OperationsPerInvoke = 3)]
        [BenchmarkCategory("register", "type")]
        public virtual object RegisterType()
        {
            Container.RegisterType(typeof(object), (ITypeLifetimeManager)Manager);
            Container.RegisterType(typeof(object), Name, (ITypeLifetimeManager)Manager);
            Container.RegisterType(typeof(string), "string", (ITypeLifetimeManager)Manager);

            return Container;
        }

        [Benchmark(Description = "RegisterInstance()", OperationsPerInvoke = 3)]
        [BenchmarkCategory("register", "instance")]
        public virtual object RegisterInstance()
        {
            Container.RegisterInstance(typeof(object), Instance1, (IInstanceLifetimeManager)Manager);
            Container.RegisterInstance(typeof(object), Name, Instance2, (IInstanceLifetimeManager)Manager);
            Container.RegisterInstance(typeof(string), "string", Instance3, (IInstanceLifetimeManager)Manager);

            return Container;
        }

        [Benchmark(Description = "RegisterFactory()", OperationsPerInvoke = 3)]
        [BenchmarkCategory("register", "factory")]
        public virtual object RegisterFactory()
        {
            Container.RegisterFactory(typeof(object), Factory, (IFactoryLifetimeManager)Manager);
            Container.RegisterFactory(typeof(object), Name, Factory, (IFactoryLifetimeManager)Manager);
            Container.RegisterFactory(typeof(string), "string", Factory, (IFactoryLifetimeManager)Manager);

            return Container;
        }
    }
}
