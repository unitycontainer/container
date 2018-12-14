using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity.Builder;
using Unity.Builder.Selection;
using Unity.Builder.Strategy;
using Unity.Extension;
using Unity.Policy;
using Unity.Resolution;
using Unity.ResolverPolicy;
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

        [TestMethod]
        public void ResolverWithElementsReturnsLiteralElements()
        {
            IUnityContainer container = new UnityContainer();
            object o1 = new object();
            object o2 = new object();
            object o3 = new object();

            container
                .RegisterInstance<object>("o1", o1)
                .RegisterInstance<object>("o2", o2);

            ResolvedArrayWithElementsResolvePolicy resolve
                = new ResolvedArrayWithElementsResolvePolicy(
                    typeof(object),
                    new LiteralValueDependencyResolvePolicy(o1),
                    new LiteralValueDependencyResolvePolicy(o3));
            container.AddExtension(new InjectedObjectConfigurationExtension(resolve));

            object[] results = (object[])container.Resolve<InjectedObject>().InjectedValue;

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Length);
            Assert.AreSame(o1, results[0]);
            Assert.AreSame(o3, results[1]);
        }

        [TestMethod]
        public void ResolverWithElementsReturnsResolvedElements()
        {
            IUnityContainer container = new UnityContainer();
            object o1 = new object();
            object o2 = new object();
            object o3 = new object();

            container
                .RegisterInstance<object>("o1", o1)
                .RegisterInstance<object>("o2", o2);

            ResolvedArrayWithElementsResolvePolicy resolve
                = new ResolvedArrayWithElementsResolvePolicy(
                    typeof(object),
                    new NamedTypeDependencyResolvePolicy(typeof(object), "o1"),
                    new NamedTypeDependencyResolvePolicy(typeof(object), "o2"));
            container.AddExtension(new InjectedObjectConfigurationExtension(resolve));

            object[] results = (object[])container.Resolve<InjectedObject>().InjectedValue;

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Length);
            Assert.AreSame(o1, results[0]);
            Assert.AreSame(o2, results[1]);
        }

        [TestMethod]
        public void ResolverWithElementsReturnsResolvedElementsForBaseClass()
        {
            IUnityContainer container = new UnityContainer();
            ILogger o1 = new MockLogger();
            ILogger o2 = new SpecialLogger();

            container
                .RegisterInstance<ILogger>("o1", o1)
                .RegisterInstance<ILogger>("o2", o2);

            ResolvedArrayWithElementsResolvePolicy resolve
                = new ResolvedArrayWithElementsResolvePolicy(
                    typeof(ILogger),
                    new NamedTypeDependencyResolvePolicy(typeof(ILogger), "o1"),
                    new NamedTypeDependencyResolvePolicy(typeof(ILogger), "o2"));
            container.AddExtension(new InjectedObjectConfigurationExtension(resolve));

            ILogger[] results = (ILogger[])container.Resolve<InjectedObject>().InjectedValue;

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Length);
            Assert.AreSame(o1, results[0]);
            Assert.AreSame(o2, results[1]);
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
                                     typeof(IConstructorSelectorPolicy),
                                     new InjectedObjectSelectorPolicy(this._resolvePolicy));
            }
        }

        private class InjectedObjectSelectorPolicy : IConstructorSelectorPolicy
        {
            private readonly IResolve _resolvePolicy;

            public InjectedObjectSelectorPolicy(IResolve resolvePolicy)
            {
                this._resolvePolicy = resolvePolicy;
            }

            public object SelectConstructor<TContext>(ref TContext context)
                where TContext : IBuilderContext
            {
                var ctr = typeof(InjectedObject).GetMatchingConstructor(new[] { typeof(object) });
                var selectedConstructor = new SelectedConstructor(ctr);
                selectedConstructor.AddParameterResolver(this._resolvePolicy);

                return selectedConstructor;
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

        public override void PreBuildUp<TBuilderContext>(ref TBuilderContext context)
        {
            if ((NamedTypeBuildKey)context.BuildKey == NamedTypeBuildKey.Make<IUnityContainer>())
            {
                context.Existing = this.container;
                context.BuildComplete = true;
            }
        }
    }
}
