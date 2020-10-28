using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
#if NET462
using Microsoft.Practices.Unity;
#else
using Unity.Injection;
using Unity.Lifetime;
using Unity;
#endif

namespace Unity.Benchmarks
{
    public class ContainerAPI
    {
        protected IUnityContainer Container;
        protected Consumer Consumer = new Consumer();
        protected static string Name = "name";

        protected static IInstanceLifetimeManager Manager = new ContainerControlledLifetimeManager();
        private static IEnumerable<TypeInfo> DefinedTypes = Assembly.GetAssembly(typeof(int)).DefinedTypes.Take(1000);
        protected static string[] TestNames = Enumerable.Repeat<string>(null, 4)
                                                        .Concat(DefinedTypes.Take(5).Select(t => t.Name))
                                                        .Concat(DefinedTypes.Select(t => t.Name).Distinct().Take(100))
                                                        .ToArray();
        protected static Type[] TestTypes = DefinedTypes.Where(t => t != typeof(IServiceProvider))
                                                        .ToArray();
        static int size = 0;
        static int position = 0;
        protected static object Instance = new object();
        protected static RegistrationDescriptor[] RegistrationsData = TestNames.Select(name =>
        {
            var types = new Type[(++size & 0x7F)];

            Array.Copy(TestTypes, position, types, 0, types.Length);
            position = (position + types.Length) & 0x4FF;

            return new RegistrationDescriptor(Instance, name, Manager, types);
        }).ToArray();


        [GlobalSetup]
        public virtual void GlobalSetup() 
            => Container = new UnityContainer().Register(RegistrationsData);


        [Benchmark(Description = "new UnityContainer()")]
        [BenchmarkCategory("new")]
        public object NewUnityContainer() 
            => new UnityContainer();


        [Benchmark(Description = "CreateChildContainer()")]
        [BenchmarkCategory("new", "child")]
        public object NewChildContainer() 
            => Container.CreateChildContainer();


        [Benchmark(Description = "Container.Registrations")]
        [BenchmarkCategory("registrations")]
        public object Registrations() 
            => Container.Registrations;


        [Benchmark(Description = "Registrations.ToArray(6351)")]
        [BenchmarkCategory("registrations", "ToArray")]
        public void RegistrationsToArray()
            => Container.Registrations.Consume(Consumer);

        [Benchmark(Description = "Registrations.PerRegistration", OperationsPerInvoke = 6351)]
        [BenchmarkCategory("registrations", "ToArray")]
        public void RegistrationsPerRegistration() 
            => Container.Registrations.Consume(Consumer);


        [Benchmark(Description = "Container.IsRegistered(true)")]
        [BenchmarkCategory("check", "true")]
        public virtual bool IsRegistered() 
            => Container.IsRegistered(typeof(IUnityContainer), null);


        [Benchmark(Description = "Container.IsRegistered(false)")]
        [BenchmarkCategory("check", "false")]
        public virtual bool IsNotRegistered() 
            => Container.IsRegistered(typeof(object), null);
    }
}
