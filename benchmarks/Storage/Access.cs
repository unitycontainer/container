using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using System.Threading;

namespace Unity.Benchmarks.Storage
{
    [ShortRunJob]
    public class Access1
    {
        private Type[] Data;
        private ThreadLocal<Type[]> threadLocal;
        private AsyncLocal<Type[]>  AsyncLocal;
        
        [GlobalSetup]
        public void GlobalSetup()
        {
            Data = typeof(Type).Assembly
                               .DefinedTypes
                               .Take(100)
                               .ToArray();

            threadLocal = new ThreadLocal<Type[]>();
            AsyncLocal = new AsyncLocal<Type[]>();
        }


        [Benchmark]
        public object ThreadLocalGet()
        {
            return threadLocal.Value;
        }

        [Benchmark]
        public object ThreadLocalSet()
        {
            threadLocal.Value = Data;
            return threadLocal.Value;
        }

        [Benchmark]
        public object AsyncLocalGet()
        {
            return AsyncLocal.Value;
        }

        [Benchmark]
        public object AsyncLocalSet()
        {
            AsyncLocal.Value = Data;
            return AsyncLocal.Value;
        }

    }
}
