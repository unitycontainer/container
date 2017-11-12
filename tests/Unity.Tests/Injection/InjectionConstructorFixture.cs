// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Injection;
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
        public void InjectionConstructor_IncorrectType()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
                _container.RegisterType<ObjectWithAmbiguousConstructors>(
                    new InjectionConstructor(typeof(int))));
        }

        [TestMethod]
        public void InjectionConstructor_IncorrectValue()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
                _container.RegisterType<ObjectWithAmbiguousConstructors>(
                    new InjectionConstructor(0)));
        }

        [TestMethod]
        public void InjectionConstructor_DefaultConstructor()
        {
            _container.RegisterType<ObjectWithAmbiguousConstructors>(new InjectionConstructor());
            Assert.AreEqual(ObjectWithAmbiguousConstructors.One, _container.Resolve<ObjectWithAmbiguousConstructors>().Signature);
        }

        [TestMethod]
        public void InjectionConstructor_SelectByValues()
        {
            _container.RegisterType<ObjectWithAmbiguousConstructors>(new InjectionConstructor(0, string.Empty, 0.0f));
            Assert.AreEqual(ObjectWithAmbiguousConstructors.Two, _container.Resolve<ObjectWithAmbiguousConstructors>().Signature);
        }

        [TestMethod]
        public void InjectionConstructor_SelectByValueTypes()
        {
            _container.RegisterType<ObjectWithAmbiguousConstructors>(new InjectionConstructor(new InjectionParameter(typeof(string)), 
                                                                                              new InjectionParameter(typeof(string)), 
                                                                                              new InjectionParameter(typeof(int))));
            Assert.AreEqual(ObjectWithAmbiguousConstructors.Three, _container.Resolve<ObjectWithAmbiguousConstructors>().Signature);
        }


        [TestMethod]
        public void InjectionConstructor_SelectAndResolveByValue()
        {
            _container.RegisterInstance(ObjectWithAmbiguousConstructors.Four);
            _container.RegisterType<ObjectWithAmbiguousConstructors>(new InjectionConstructor(new ResolvedParameter(typeof(string)), 
                                                                                              string.Empty, 
                                                                                              string.Empty));
            Assert.AreEqual(ObjectWithAmbiguousConstructors.Four, _container.Resolve<ObjectWithAmbiguousConstructors>().Signature);
        }


        [TestMethod]
        public void InjectionConstructor_ResolveNamedTypeArgument()
        {
            _container.RegisterInstance(ObjectWithAmbiguousConstructors.Four);
            _container.RegisterInstance(ObjectWithAmbiguousConstructors.Five, ObjectWithAmbiguousConstructors.Five);

            _container.RegisterType<ObjectWithAmbiguousConstructors>(new InjectionConstructor(typeof(string), 
                                                                                              typeof(string), 
                                                                                              typeof(IUnityContainer)));
            Assert.AreEqual(ObjectWithAmbiguousConstructors.Five, _container.Resolve<ObjectWithAmbiguousConstructors>().Signature);
        }
    }
}
