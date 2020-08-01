using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Lifetime;

namespace Unity.Benchmarks.Container
{
    [GcForce(true)]
    public class RegisterAllocation
    {
        #region Fields

        protected static IInstanceLifetimeManager Manager = new ContainerControlledLifetimeManager();
        private static IEnumerable<TypeInfo> DefinedTypes = Assembly.GetAssembly(typeof(int)).DefinedTypes;
        protected static string[] TestNames = Enumerable.Repeat<string>(null, 4)
                                                        .Concat(DefinedTypes.Take(5).Select(t => t.Name))
                                                        .Concat(DefinedTypes.Take(1000).Select(t => t.Name))
                                                        .ToArray();
        protected static Type[] TestTypes = DefinedTypes.Where(t => t != typeof(IServiceProvider))
                                                        .ToArray();
        static int size = 0;
        static int position = 0;
        protected static object Instance = new object();
        protected static RegistrationDescriptor[] Registrations = TestNames.Select(name =>
            {
                var types = new Type[(++size & 0x7F)];

                Array.Copy(TestTypes, position, types, 0, types.Length);
                position = (position + types.Length) & 0x7FF;

                return new RegistrationDescriptor(Instance, name, Manager, types);
            }).ToArray();

        #endregion

        [Benchmark(Description = "Register()")]
        [BenchmarkCategory("register", "allocation")]
        public object RegisterDynamic()
        {
            var container = new UnityContainer();
            container.Register(Registrations);

            return container;
        }

        [Benchmark(Description = "Preallocated.Register()", Baseline = true)]
        [BenchmarkCategory("register", "allocation")]
        public object RegisterPreallocated()
        {
            var container = new UnityContainer(65000);
            container.Register(Registrations);

            return container;
        }
    }
}
