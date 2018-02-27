using System.Linq;
using BenchmarkDotNet.Attributes;
using Runner.Setup;
using System.Collections.Generic;

namespace Runner.Tests
{
    [BenchmarkCategory("Registration")]
    [Config(typeof(BenchmarkConfiguration))]
    public class Registration
    {
        //private const string TestName = "Test Name";

        //[Benchmark(Description = "Register Type")]
        //public void RegisterType() => 
        //    Adapter.RegisterType(typeof(Registration), null);

        //[Benchmark(Description = "Register Type (Singleton)")]
        //public void RegisterTypeSingleton() => 
        //    Adapter.RegisterTypeSingleton(typeof(Registration), null);

        //[Benchmark(Description = "Register Named Type")]
        //public void RegisterNamedType() => 
        //    Adapter.RegisterType(typeof(Registration), TestName);

        //[Benchmark(Description = "Register Named Type (Singleton)")]
        //public void RegisterNamedTypeSingleton() => 
        //    Adapter.RegisterTypeSingleton(typeof(Registration), TestName);


        //[Benchmark(Description = "Register Type Mapping")]
        //public void RegisterTypeMapping() => 
        //    Adapter.RegisterTypeMapping(typeof(TestsBase), typeof(Registration), null);

        //[Benchmark(Description = "Register Type Mapping (Singleton)")]
        //public void RegisterTypeMappingSingleton() => 
        //    Adapter.RegisterTypeMappingSingleton(typeof(TestsBase), typeof(Registration), null);

        //[Benchmark(Description = "Register Named Type Mapping")]
        //public void RegisterNamedTypeMapping() => 
        //    Adapter.RegisterTypeMapping(typeof(TestsBase), typeof(Registration), TestName);

        //[Benchmark(Description = "Register Named Type Mapping (Singleton)")]
        //public void RegisterNamedTypeMappingSingleton() => 
        //    Adapter.RegisterTypeMappingSingleton(typeof(TestsBase), typeof(Registration), TestName);
    }
}
