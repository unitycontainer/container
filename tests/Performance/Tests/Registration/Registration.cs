using BenchmarkDotNet.Attributes;

namespace Performance.Tests
{
    public class Registration : RegistrationBase
    {
        [Benchmark(Description = "Register (No Mapping)", OperationsPerInvoke = 100)]
        public override object Register() => base.Register();

        [Benchmark(Description = "Register Mapping", OperationsPerInvoke = 100)]
        public override object RegisterMapping() => base.RegisterMapping();

        [Benchmark(Description = "Register Instance", OperationsPerInvoke = 100)]
        public override object RegisterInstance() => base.RegisterInstance();

        [Benchmark(Description = "Registrations.ToArray(100)", OperationsPerInvoke = 100)]
        public override object Registrations() => base.Registrations();

        [Benchmark(Description = "IsRegistered (True)", OperationsPerInvoke = 100)]
        public override object IsRegistered() => base.IsRegistered();

        [Benchmark(Description = "IsRegistered (False)", OperationsPerInvoke = 100)]
        public override object IsRegisteredFalse() => base.IsRegisteredFalse();
    }
}
