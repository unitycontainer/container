using System;
using BenchmarkDotNet.Attributes;
using Runner.Tests;
using Unity;
using Unity.Lifetime;

namespace Performance.Tests
{
    [BenchmarkCategory("ChildContainer")]
    [MemoryDiagnoser]
    public class ChildContainer
    {
        private IUnityContainer _container;

        [Params(1, 2, 3, 4, 10, 50, 100)] public int Resolves;

        [GlobalSetup]
        public void SetupContainer()
        {
            _container = new UnityContainer();
            _container.RegisterType<Poco>();
            _container.RegisterType<IService, Service>();
            _container.RegisterType<IService, Service>("1");
            _container.RegisterType<IService, Service>("2");

            for (var i = 0; i < 100; i++)
                _container.RegisterType<DisposableObject>(i.ToString(), new HierarchicalLifetimeManager());
        }

        [Benchmark(Baseline = true)]
        public void CreateChildContainer()
        {
            using (_container.CreateChildContainer())
            {
            }
        }

        //[Benchmark]
        //public void ChildTransient()
        //{
        //    using (var child = _container.CreateChildContainer())
        //    {
        //        for (var i = 0; i < Resolves; i++)
        //            child.Resolve(typeof(Poco), null);
        //    }
        //}

        //[Benchmark]
        //public void ChildMapping()
        //{
        //    using (var child = _container.CreateChildContainer())
        //    {
        //        for (var i = 0; i < Resolves; i++)
        //            child.Resolve(typeof(IService), null);
        //    }
        //}

        //[Benchmark]
        //public void ChildArray()
        //{
        //    using (var child = _container.CreateChildContainer())
        //    {
        //        for (var i = 0; i < Resolves; i++)
        //            child.Resolve(typeof(IService[]), null);
        //    }
        //}

        //[Benchmark]
        //public void ChildEnumerable()
        //{
        //    using (var child = _container.CreateChildContainer())
        //    {
        //        for (var i = 0; i < Resolves; i++)
        //            child.Resolve(typeof(IEnumerable<IService>), null);
        //    }
        //}

        [Benchmark]
        public void ChildDisposableObject()
        {
            using (var child = _container.CreateChildContainer())
            {
                for (var i = 0; i < Resolves; i++)
                    child.Resolve<DisposableObject>(i.ToString());
            }
        }


        private class DisposableObject : IDisposable
        {
            public bool WasDisposed = false;

            public virtual void Dispose()
            {
                WasDisposed = true;
            }
        }
    }
}
