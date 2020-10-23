using BenchmarkDotNet.Attributes;
using System;
using Unity.Resolution;
#if NET462
using Microsoft.Practices.Unity;
#else
using Unity.Injection;
using Unity.Lifetime;
using Unity;
#endif

namespace Unity.Benchmarks
{
    public class RegisterAPI
    {
        protected static LifetimeManager Manager1;
        protected static LifetimeManager Manager2;
        protected static LifetimeManager Manager3;
        protected static IUnityContainer Container;
        protected static Func<IUnityContainer, Type, string, object> Factory = (c, t, n) => c;
        protected const string Name = "name";
        protected object Instance1 = new object();
        protected object Instance2 = new object();
        protected object Instance3 = "object";
#if NET462
        protected InjectionFactory InjectionFactory = new InjectionFactory(Factory);
#endif


        //[IterationSetup]
        public virtual void IterationSetup()
        { 
            Container = new UnityContainer();
            Manager1 = new ContainerControlledLifetimeManager();
            Manager2 = new ContainerControlledLifetimeManager();
            Manager3 = new ContainerControlledLifetimeManager();
        }

        [Benchmark(Description = "RegisterType()", OperationsPerInvoke = 3)]
        [BenchmarkCategory("register", "type")]
        public virtual object RegisterType()
        {
#if NET462
            Container.RegisterType(typeof(object), Manager1);
            Container.RegisterType(typeof(object), Name, Manager2);
            Container.RegisterType(typeof(string), "string", Manager3);
#else
            Container.RegisterType(typeof(object), (ITypeLifetimeManager)Manager1);
            Container.RegisterType(typeof(object), Name, (ITypeLifetimeManager)Manager2);
            Container.RegisterType(typeof(string), "string", (ITypeLifetimeManager)Manager3);
#endif

            return Container;
        }

        [Benchmark(Description = "RegisterInstance()", OperationsPerInvoke = 3)]
        [BenchmarkCategory("register", "instance")]
        public virtual object RegisterInstance()
        {
#if NET462
            Container.RegisterInstance(typeof(object), Instance1, Manager1);
            Container.RegisterInstance(typeof(object), Name, Instance2, Manager2);
            Container.RegisterInstance(typeof(string), "string", Instance3, Manager3);
#else
            Container.RegisterInstance(typeof(object), Instance1, (IInstanceLifetimeManager)Manager1);
            Container.RegisterInstance(typeof(object), Name, Instance2, (IInstanceLifetimeManager)Manager2);
            Container.RegisterInstance(typeof(string), "string", Instance3, (IInstanceLifetimeManager)Manager3);
#endif

            return Container;
        }

        [Benchmark(Description = "RegisterFactory()", OperationsPerInvoke = 3)]
        [BenchmarkCategory("register", "factory")]
        public virtual object RegisterFactory()
        {
#if NET462
            Container.RegisterType(typeof(object), Manager1, InjectionFactory);
            Container.RegisterType(typeof(object), Name, Manager2, InjectionFactory);
            Container.RegisterType(typeof(string), "string", Manager3, InjectionFactory);
#else
            Container.RegisterFactory(typeof(object), Factory, (IFactoryLifetimeManager)Manager1);
            Container.RegisterFactory(typeof(object), Name, Factory, (IFactoryLifetimeManager)Manager2);
            Container.RegisterFactory(typeof(string), "string", Factory, (IFactoryLifetimeManager)Manager3);

#endif
            return Container;
        }
    }
}
