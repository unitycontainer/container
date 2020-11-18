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


        private PropertyInfo property = typeof(Access1).GetProperty(nameof(Property));
        private PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(Access1))[nameof(Property)];

        [Import]
        [DefaultValue(0)]
        public int Property { get; set; }

        [Benchmark]
        public object GetCustomAttributes()
        {
            return property.GetCustomAttributes(true);
        }


        [Benchmark]
        public object GetCustomAttributes_Type()
        {
            return property.GetCustomAttributes(typeof(ImportAttribute), true);
        }


        [Benchmark]
        public object GetCustomAttributeData()
        {
            return CustomAttributeData.GetCustomAttributes(property);
        }

    }
}
