using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using System.Threading.Tasks;
using Unity.BuiltIn;
using Unity.Lifetime;

namespace Unity.Benchmarks.Scope
{
    [CategoriesColumn]
    [BenchmarkCategory("Scope")]
    public class ScopeBenchmarks
    {
        Container.Scope scope;
        LifetimeManager lifetime;
        Type[] registerAs;
        Type[][] registerArrays;

        [GlobalSetup]
        public void GlobalSetup()
        {
            lifetime = new TransientLifetimeManager();
            registerAs = typeof(Type).Assembly
                                   .DefinedTypes
                                   .Take(1000)
                                   .ToArray();

            registerArrays = typeof(Type).Assembly
                                   .DefinedTypes
                                   .Take(100)
                                   .Select(t => new[] { t })
                                   .ToArray();
        }

        [Benchmark]
        [BenchmarkCategory("create")]
        public Container.Scope ContainerScope() => new ContainerScope();


        [Benchmark]
        [BenchmarkCategory("create", "data")]
        public RegistrationData RegistrationData() => new RegistrationData(lifetime, registerAs);

        [Benchmark]
        [BenchmarkCategory("create", "data")]
        public RegistrationData RegistrationNamedData() => new RegistrationData(lifetime, registerAs);

        [Benchmark]
        [BenchmarkCategory("register")]
        public Container.Scope RegisterType()
        {
            scope = ContainerScope();
            RegistrationData data = new RegistrationData(lifetime, registerAs);

            scope.Add(in data);
            
            return scope;
        }

        [Benchmark]
        [Arguments(1)]
        [Arguments(4)]
        [Arguments(8)]
        [Arguments(16)]
        [BenchmarkCategory("register", "parallel")]
        public int RegisterTypeParallel(int threads)
        {
            scope = ContainerScope();

            Parallel.ForEach(registerArrays, new ParallelOptions() { MaxDegreeOfParallelism = threads }, (array) =>
            {
                RegistrationData data = new RegistrationData(lifetime, array);
                scope.Add(in data);
            });

            return threads;
        }


        [Benchmark(Baseline = true)]
        [Arguments(1)]
        [Arguments(4)]
        [Arguments(8)]
        [Arguments(16)]
        [BenchmarkCategory("register", "parallel")]
        public int RegisterTypeParallelBaseline(int threads)
        {
            scope = ContainerScope();
            int code = 0;

            Parallel.ForEach(registerArrays, new ParallelOptions() { MaxDegreeOfParallelism = threads }, (array) =>
            {
                RegistrationData data = new RegistrationData(lifetime, array);
                code += null != data.Manager ? 1 : 0;
            });

            return code;
        }


        [Benchmark]
        [BenchmarkCategory("register")]
        public Container.Scope RegisterNamedType()
        {
            scope = ContainerScope();
            RegistrationData data = new RegistrationData("name", lifetime, registerAs);

            scope.Add(in data);

            return scope;
        }
    }
}
