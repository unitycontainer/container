using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
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


        private PropertyInfo property = typeof(Access1).GetTypeInfo().GetProperty(nameof(Property));

        [Import]
        public int Property { get; set; }

        [Benchmark(Description = "PropertyInfo.GetCustomAttributes(true)")]
        public object GetCustomAttributesAll()
        {
            return property.GetCustomAttributes(true);
        }

        [Benchmark(Description = "PropertyInfo.GetCustomAttribute(type, true)")]
        public object GetCustomAttribute()
        {
            return property.GetCustomAttribute(typeof(ImportAttribute), true);
        }

        [Benchmark(Description = "PropertyInfo.GetCustomAttributes(type, true)")]
        public object GetCustomAttributesType()
        {
            return property.GetCustomAttributes(typeof(ImportAttribute), true);
        }
    }
}
