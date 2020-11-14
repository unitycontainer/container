using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Container;

namespace Unity.Benchmarks.Storage
{
    [ShortRunJob]
    public class Access1
    {
        private object NullValue = null;
        private object Value = new object();
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
        public object NoValueReference()
        {
            return ReferenceEquals(RegistrationManager.NoValue, this);
        }

        object isNo = RegistrationManager.NoValue;

        [Benchmark]
        public object NoValueIs()
        {
            return isNo.IsNoValue();
        }

        [Benchmark]
        public object NoValueIsNot()
        {
            return Value.IsNoValue();
        }

        [Benchmark]
        public object NoValueEquals()
        {
            return RegistrationManager.NoValue.Equals(this);
        }

    }
}
