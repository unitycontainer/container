// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Builder;
using Unity.Builder.Selection;
using Unity.Injection;
using Unity.Policy;
using Unity.ResolverPolicy;
using Unity.Tests.TestObjects;

namespace Unity.Tests.Injection
{
    [TestClass]
    public class InjectionConstructorFixture
    {
        private IUnityContainer _container;

        [TestInitialize]
        public void Setup()
        {
            _container = new UnityContainer();
        }

        [TestMethod]
        public void InjectionConstructorIncorrectType()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
                _container.RegisterType<ObjectWithAmbiguousConstructors>(
                    new InjectionConstructor(new Type[] {typeof(int)})));
        }

        [TestMethod]
        public void InjectionConstructorIncorrectValue()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
                _container.RegisterType<ObjectWithAmbiguousConstructors>(
                    new InjectionConstructor(new object[] { 0 })));
        }

        [TestMethod]
        public void InjectionConstructorDefault()
        {
            _container.RegisterType<ObjectWithAmbiguousConstructors>(new InjectionConstructor());
            Assert.AreEqual(ObjectWithAmbiguousConstructors.One, _container.Resolve<ObjectWithAmbiguousConstructors>().Signature);
        }

        [TestMethod]
        public void InjectionConstructorTwoArg()
        {
            _container.RegisterType<ObjectWithAmbiguousConstructors>(new InjectionConstructor(typeof(int), typeof(string), typeof(float)));
            Assert.AreEqual(ObjectWithAmbiguousConstructors.Two, _container.Resolve<ObjectWithAmbiguousConstructors>().Signature);
        }

        [TestMethod]
        public void InjectionConstructorThreeArg()
        {
            _container.RegisterType<ObjectWithAmbiguousConstructors>(new InjectionConstructor(typeof(string), typeof(string), typeof(int)));
            Assert.AreEqual(ObjectWithAmbiguousConstructors.Three, _container.Resolve<ObjectWithAmbiguousConstructors>().Signature);
        }

        [TestMethod]
        public void InjectionConstructorInsertsChooserForDefaultConstructor()
        {
            var ctor = new InjectionConstructor();
            var context = new MockBuilderContext
                {
                    BuildKey = new NamedTypeBuildKey(typeof(GuineaPig))
                };
            IPolicyList policies = context.PersistentPolicies;

            ctor.AddPolicies(typeof(GuineaPig), policies);

            var selector = policies.Get<IConstructorSelectorPolicy>(
                new NamedTypeBuildKey(typeof(GuineaPig)));

            SelectedConstructor selected = selector.SelectConstructor(context, policies);
            Assert.AreEqual(typeof(GuineaPig).GetMatchingConstructor(new Type[0]), selected.Constructor);
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
                    BuildKey = new NamedTypeBuildKey(typeof(GuineaPig))
                };
            IPolicyList policies = context.PersistentPolicies;

            ctor.AddPolicies(typeof(GuineaPig), policies);

            var selector = policies.Get<IConstructorSelectorPolicy>(
                new NamedTypeBuildKey(typeof(GuineaPig)));

            SelectedConstructor selected = selector.SelectConstructor(context, policies);
            var resolvers = selected.GetParameterResolvers();

            Assert.AreEqual(typeof(GuineaPig).GetMatchingConstructor(Sequence.Collect(typeof(string), typeof(int))), selected.Constructor);
            Assert.AreEqual(2, resolvers.Length);

            Assert.AreEqual(expectedString, (string)resolvers[0].Resolve(null));
            Assert.AreEqual(expectedInt, (int)resolvers[1].Resolve(null));
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
            var resolvers = selected.GetParameterResolvers();

            Assert.AreEqual(typeof(GuineaPig).GetMatchingConstructor(Sequence.Collect(typeof(string), typeof(ILogger))), selected.Constructor);
            Assert.AreEqual(2, resolvers.Length);

            var policy = resolvers[1];
            Assert.IsTrue(policy is NamedTypeDependencyResolverPolicy);
        }

        [TestMethod]
        public void InjectionConstructorThrowsIfNoMatchingConstructor()
        {
            InjectionConstructor ctor = new InjectionConstructor(typeof(double));
            var context = new MockBuilderContext();

            AssertExtensions.AssertException<InvalidOperationException>(
                () => ctor.AddPolicies(typeof(GuineaPig), context.PersistentPolicies));
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
