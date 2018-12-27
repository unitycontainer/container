using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;
using Unity.Extension;
using Unity.Injection;
using Unity.Policy;
using Unity.Strategies;
using Unity.Tests.v5.TestSupport;

namespace Unity.Tests.v5
{
    [TestClass]
    public class ResolvingArraysFixture
    {
        [TestMethod]
        public void ResolveAllReturnsRegisteredObjects()
        {
            IUnityContainer container = new UnityContainer();
            object o1 = new object();
            object o2 = new object();

            container
                .RegisterInstance<object>("o1", o1)
                .RegisterInstance<object>("o2", o2);

            List<object> results = new List<object>(container.ResolveAll<object>());

            CollectionAssertExtensions.AreEqual(new object[] { o1, o2 }, results);
        }

        [TestMethod]
        public void ResolveAllReturnsRegisteredObjectsForBaseClass()
        {
            IUnityContainer container = new UnityContainer();
            ILogger o1 = new MockLogger();
            ILogger o2 = new SpecialLogger();

            container
                .RegisterInstance<ILogger>("o1", o1)
                .RegisterInstance<ILogger>("o2", o2);

            List<ILogger> results = new List<ILogger>(container.ResolveAll<ILogger>());
            CollectionAssertExtensions.AreEqual(new ILogger[] { o1, o2 }, results);
        }

        private class InjectedObjectConfigurationExtension : UnityContainerExtension
        {
            private readonly IResolve _resolvePolicy;

            public InjectedObjectConfigurationExtension(IResolve resolvePolicy)
            {
                this._resolvePolicy = resolvePolicy;
            }

            protected override void Initialize()
            {
                Context.Policies.Set(typeof(InjectedObject), null, 
                                     typeof(ISelect<ConstructorInfo>),
                                     new InjectedObjectSelectorPolicy(this._resolvePolicy));
            }
        }

        private class InjectedObjectSelectorPolicy : ISelect<ConstructorInfo>
        {
            private readonly IResolve _resolvePolicy;

            public InjectedObjectSelectorPolicy(IResolve resolvePolicy)
            {
                this._resolvePolicy = resolvePolicy;
            }

            public IEnumerable<object> Select(ref BuilderContext context)
            {
                var ctr = typeof(InjectedObject).GetMatchingConstructor(new[] { typeof(object) });

                return new []{ new InjectionConstructor(ctr, _resolvePolicy) } ;
            }
        }

        public class InjectedObject
        {
            public readonly object InjectedValue;

            public InjectedObject(object injectedValue)
            {
                this.InjectedValue = injectedValue;
            }
        }

        public class SimpleClass
        {
        }
    }

    internal class ReturnContainerStrategy : BuilderStrategy
    {
        private IUnityContainer container;

        public ReturnContainerStrategy(IUnityContainer container)
        {
            this.container = container;
        }

        public override void PreBuildUp(ref BuilderContext context)
        {
            if (typeof(IUnityContainer) == context.Type)
            {
                context.Existing = container;
                context.BuildComplete = true;
            }
        }
    }
}
