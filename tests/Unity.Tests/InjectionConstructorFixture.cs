using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Builder;
using Unity.Builder.Selection;
using Unity.Injection;
using Unity.Policy;
using Unity.ResolverPolicy;
using Unity.Tests.v5.TestSupport;

namespace Unity.Tests.v5
{
    [TestClass]
    public class InjectionConstructorFixture
    {
        [TestMethod]
        public void InjectionConstructorInsertsChooserForDefaultConstructor()
        {
            var ctor = new InjectionConstructor();
            var context = new MockBuilderContext
                {
                    BuildKey = new NamedTypeBuildKey(typeof (GuineaPig))
                };
            IPolicyList policies = context.PersistentPolicies;

            ctor.AddPolicies(typeof(GuineaPig), policies);

            var selector = policies.Get<IConstructorSelectorPolicy>(
                new NamedTypeBuildKey(typeof(GuineaPig)));

            SelectedConstructor selected = selector.SelectConstructor(context, policies);
            Assert.AreEqual(typeof(GuineaPig).GetConstructor(new Type[0]), selected.Constructor);
            Assert.AreEqual(0, selected.GetParameterResolvers().Length);
        }

        [TestMethod]
        public void InjectionConstructorInsertsChooserForConstructorWithParameters()
        {
            string expectedString = "Hello";
            int expectedInt = 12;

            var ctor = new InjectionConstructor(expectedString, expectedInt);
            var context = new MockBuilderContext
                {
                    BuildKey = new NamedTypeBuildKey(typeof (GuineaPig))
                };
            IPolicyList policies = context.PersistentPolicies;

            ctor.AddPolicies(typeof(GuineaPig), policies);

            var selector = policies.Get<IConstructorSelectorPolicy>(
                new NamedTypeBuildKey(typeof(GuineaPig)));

            SelectedConstructor selected = selector.SelectConstructor(context, policies);
            var keys = selected.GetParameterResolvers();

            Assert.AreEqual(typeof(GuineaPig).GetConstructor(Sequence.Collect(typeof(string), typeof(int))), selected.Constructor);
            Assert.AreEqual(2, keys.Length);

            Assert.AreEqual(expectedString, (string)keys[0].Resolve(context));
            Assert.AreEqual(expectedInt, (int)keys[1].Resolve(context));
        }

        [TestMethod]
        public void InjectionConstructorSetsResolverForInterfaceToLookupInContainer()
        {
            var ctor = new InjectionConstructor("Logger", typeof(ILogger));
            var context = new MockBuilderContext();
            context.BuildKey = new NamedTypeBuildKey(typeof(GuineaPig));
            IPolicyList policies = context.PersistentPolicies;

            ctor.AddPolicies(typeof(GuineaPig), policies);

            var selector = policies.Get<IConstructorSelectorPolicy>(
                new NamedTypeBuildKey(typeof(GuineaPig)));

            SelectedConstructor selected = selector.SelectConstructor(context, policies);
            var keys = selected.GetParameterResolvers();

            Assert.AreEqual(typeof(GuineaPig).GetConstructor(Sequence.Collect(typeof(string), typeof(ILogger))), selected.Constructor);
            Assert.AreEqual(2, keys.Length);

            Assert.IsTrue(keys[1] is NamedTypeDependencyResolverPolicy);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InjectionConstructorThrowsIfNoMatchingConstructor()
        {
            InjectionConstructor ctor = new InjectionConstructor(typeof(double));
            var context = new MockBuilderContext();

            ctor.AddPolicies(typeof(GuineaPig), context.PersistentPolicies);
        }

        private object ResolveValue(IPolicyList policies, string key)
        {
            IResolverPolicy resolver = policies.Get<IResolverPolicy>(key);
            return resolver.Resolve(null);
        }

        private class GuineaPig
        {
            public GuineaPig()
            {
            }

            public GuineaPig(int i)
            {
            }

            public GuineaPig(string s)
            {
            }

            public GuineaPig(int i, string s)
            {
            }

            public GuineaPig(string s, int i)
            {
            }

            public GuineaPig(string s, ILogger logger)
            {
            }
        }
    }
}
