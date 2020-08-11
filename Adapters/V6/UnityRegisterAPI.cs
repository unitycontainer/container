using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Unity.Benchmarks;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.v6
{
    [ShortRunJob(RuntimeMoniker.Net48)]
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    public class UnityRegisterAPI : RegisterAPI
    {
        protected static ResolveDelegate<IResolveContext> ResolveDelegate = (ref IResolveContext c) => c.Type;
        protected RegistrationDescriptor[] registrations;

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

        //[Benchmark(Description = "Register()", OperationsPerInvoke = 3)]
        //[BenchmarkCategory("register", "descriptors")]
        //public object Register()
        //    => Container.Register(registrations);
    }
}
