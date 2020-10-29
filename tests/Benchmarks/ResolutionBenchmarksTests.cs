using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity.Injection;
using Unity.Lifetime;

namespace Unity.Benchmarks
{
    [TestClass]
    public class ResolutionBenchmarksTests
    {
        #region Scaffolding

        protected IUnityContainer Container;

        [TestInitialize]
        public void GlobalSetup()
        {
            Container = new UnityContainer()
                .RegisterType(typeof(List<>), new InjectionConstructor())
                .RegisterType(typeof(List<object>))
                .RegisterType<Service>();
                //.CreateChildContainer()
                //.CreateChildContainer();
        }

        #endregion


        #region Interfaces

        [TestMethod]
        public void Resolve_IUnityContainer()
        {
            Assert.IsNotNull(Container.Resolve(typeof(IUnityContainer), null));
        }

        [TestMethod]
        public void Resolve_IServiceProvider()
        {
            Assert.IsNotNull(Container.Resolve(typeof(IServiceProvider), null));
        }

        [TestMethod]
        public void Resolve_IUnityContainerAsync()
        {
            Assert.IsNotNull(((IUnityContainerAsync)Container)
                .ResolveAsync(typeof(IUnityContainerAsync), (string)null)
                .GetAwaiter()
                .GetResult());
        }

        #endregion


        [TestMethod]
        public void Resolve_Registered()
        {
            Container.RegisterType(typeof(Service), new OnceInLifetimeManager());

            var instance = Container.Resolve(typeof(Service), null);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service));
        }

        [Ignore]
        [TestMethod]
        public void Resolve_Object()
        {
            Assert.IsNotNull(Container.Resolve(typeof(object), null));
        }

        [Ignore]
        [TestMethod]
        public void Resolve_Object_Twice()
        {
            var instance1 = Container.Resolve(typeof(object), null);
            var instance2 = Container.Resolve(typeof(object), null);

            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
        }

        [Ignore]
        [TestMethod]
        public void Resolve_Generic()
        {
            Assert.IsNotNull(Container.Resolve(typeof(List<int>), null));
        }

        [TestMethod]
        public void ResolvingFromRegistered()
        {
            var TheOtherService = new OtherService();
            var TheEveryTimeNoBuildManager = new EveryTimeNoBuildManager(TheOtherService);
            var TheOnceInLifetimeNoBuildManager = new OnceInLifetimeNoBuildManager(TheOtherService);
            var TheOnceInAWhileNoBuildManager = new OnceInAWhileNoBuildManager(TheOtherService);

            Container
                .RegisterType(typeof(Service), new EveryTimeManager())
                .RegisterType(typeof(Service), "OnceInLifetime", new OnceInLifetimeManager())
                .RegisterType(typeof(Service), "OnceInAWhile", new OnceInAWhileManager())
                .RegisterType(typeof(OtherService), TheEveryTimeNoBuildManager)
                .RegisterType(typeof(OtherService), "OnceInLifetime", TheOnceInLifetimeNoBuildManager)
                .RegisterType(typeof(OtherService), "OnceInAWhile", TheOnceInAWhileNoBuildManager);

            var instance = Container.Resolve(typeof(Service), "OnceInLifetime");

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service));
            Assert.AreNotSame(instance, Container.Resolve(typeof(Service), "OnceInLifetime"));
        }

        [TestMethod]
        public void PipelineFromRegistered()
        {
            var TheOtherService = new OtherService();
            var TheEveryTimeNoBuildManager = new EveryTimeNoBuildManager(TheOtherService);
            var TheOnceInLifetimeNoBuildManager = new OnceInLifetimeNoBuildManager(TheOtherService);
            var TheOnceInAWhileNoBuildManager = new OnceInAWhileNoBuildManager(TheOtherService);

            Container
                .RegisterType(typeof(Service), new EveryTimeManager())
                .RegisterType(typeof(Service), "OnceInLifetime", new OnceInLifetimeManager())
                .RegisterType(typeof(Service), "OnceInAWhile", new OnceInAWhileManager())
                .RegisterType(typeof(OtherService), TheEveryTimeNoBuildManager)
                .RegisterType(typeof(OtherService), "OnceInLifetime", TheOnceInLifetimeNoBuildManager)
                .RegisterType(typeof(OtherService), "OnceInAWhile", TheOnceInAWhileNoBuildManager);

            var instance = Container.Resolve(typeof(OtherService), null);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(OtherService));
            Assert.AreSame(instance, Container.Resolve(typeof(OtherService), null));
        }


        public class EveryTimeManager : LifetimeManager, ITypeLifetimeManager
        {
            public override ResolutionStyle Style => ResolutionStyle.EveryTime;

            protected override LifetimeManager OnCreateLifetimeManager() => throw new NotSupportedException();

            public override void SetValue(object newValue, ICollection<IDisposable> scope)
            { }
        }

        public class OnceInLifetimeManager : LifetimeManager, ITypeLifetimeManager
        {
            public override ResolutionStyle Style => ResolutionStyle.OnceInLifetime;

            protected override LifetimeManager OnCreateLifetimeManager() => throw new NotSupportedException();

            public override void SetValue(object newValue, ICollection<IDisposable> scope)
            { }
        }

        public class OnceInAWhileManager : LifetimeManager, ITypeLifetimeManager
        {
            public override ResolutionStyle Style => ResolutionStyle.OnceInWhile;

            protected override LifetimeManager OnCreateLifetimeManager() => throw new NotSupportedException();

            public override void SetValue(object newValue, ICollection<IDisposable> scope)
            { }
        }


        public class EveryTimeNoBuildManager : EveryTimeManager
        {
            private object _value;

            public EveryTimeNoBuildManager(object value)
            {
                _value = value;
            }

            public override object GetValue(ICollection<IDisposable> scope) => _value;
            public override object TryGetValue(ICollection<IDisposable> scope)
            {
                Pipeline = null;
                return NoValue;
            }
        }

        public class OnceInLifetimeNoBuildManager : OnceInLifetimeManager
        {
            private object _value;

            public OnceInLifetimeNoBuildManager(object value)
            {
                _value = value;
            }

            public override object GetValue(ICollection<IDisposable> scope) => _value;
            public override object TryGetValue(ICollection<IDisposable> scope)
            {
                Pipeline = null;
                return NoValue;
            }
        }

        public class OnceInAWhileNoBuildManager : OnceInAWhileManager
        {
            private object _value;

            public OnceInAWhileNoBuildManager(object value)
            {
                _value = value;
            }

            public override object GetValue(ICollection<IDisposable> scope) => _value;
            public override object TryGetValue(ICollection<IDisposable> scope)
            {
                Pipeline = null;
                return NoValue;
            }
        }


        public class Service
        {
        }
        public class OtherService
        {
        }
    }
}
