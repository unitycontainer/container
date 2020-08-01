using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.Linq;
using System.Reflection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Benchmarks
{
    [ShortRunJob(RuntimeMoniker.NetCoreApp50)]
    public class UnityRegisterAPI : RegisterAPI
    {
        protected static ResolveDelegate<IResolveContext> ResolveDelegate = (ref IResolveContext c) => c.Type;
        protected static object Instance = new object();
        protected static Type[] TestTypes;
        protected static string[] TestNames;
        protected static IInstanceLifetimeManager Manager = new ContainerControlledLifetimeManager();
        protected static RegistrationDescriptor[] registrations;
        protected static RegistrationDescriptor[] Registrations;

        [GlobalSetup]
        public static void InitializeClass()
        {
            var DefinedTypes = Assembly.GetAssembly(typeof(int)).DefinedTypes;
            TestNames = Enumerable.Repeat<string>(null, 4)
                .Concat(DefinedTypes.Take(5).Select(t => t.Name))
                .Concat(DefinedTypes.Take(100).Select(t => t.Name))
                .ToArray();
            TestTypes = DefinedTypes.Where(t => t != typeof(IServiceProvider))
                                    .ToArray();

            var size = 0;
            var position = 0;

            Registrations = TestNames.Select(name =>
            {
                var types = new Type[(++size & 0x7F)];

                Array.Copy(TestTypes, position, types, 0, types.Length);
                position = (position + types.Length) & 0x7FF;

                return new RegistrationDescriptor(Instance, name, Manager, types);
            }).ToArray();
        }

        public override void IterationSetup()
        {
            base.IterationSetup();
            registrations = new[]
            {
                new RegistrationDescriptor(typeof(object),  null, (ITypeLifetimeManager)Manager1, typeof(object)),
                new RegistrationDescriptor(new object(),    Name, (IInstanceLifetimeManager)Manager2, typeof(object)),
                new RegistrationDescriptor(ResolveDelegate, "string", (IFactoryLifetimeManager)Manager3, typeof(string))
            };
        }


        [Benchmark(Description = "RegisterBulk()")]
        [BenchmarkCategory("register", "descriptors", "bulk")]
        public object RegisterBulk()
            => Container.Register(Registrations);

        [Benchmark(Description = "Register()", OperationsPerInvoke = 3)]
        [BenchmarkCategory("register", "descriptors")]
        public object Register() 
            => Container.Register(registrations);
    }
}
