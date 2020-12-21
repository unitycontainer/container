using BenchmarkDotNet.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Unity.Benchmarks.Storage
{
    [ShortRunJob]
    public class Access1
    {
        private Func<Type, object> func = (Type type) => type.Name;

        protected virtual object method(Type type) => type.Name;

        [Benchmark]
        public object GetBenchmark_Method()
        {
            return method(typeof(Access1));

        }

        [Benchmark]
        public object GetBenchmark_Func()
        {
            return func(typeof(Access1));
        }
    }
}
