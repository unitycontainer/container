using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Tests.v5.TestSupport;

namespace Unity.Tests.v5.Injection
{
    [TestClass]
    public class InjectionMethodFixture
    {
        [TestMethod]
        public void QualifyingInjectionMethodCanBeConfiguredAndIsCalled()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<LegalInjectionMethod>(
                        new InjectionMethod("InjectMe"));

            LegalInjectionMethod result = container.Resolve<LegalInjectionMethod>();
            Assert.IsTrue(result.WasInjected);
        }

        [TestMethod]
        public void CannotConfigureGenericInjectionMethod()
        {
            AssertExtensions.AssertException<InvalidOperationException>(() =>
                {
                    new UnityContainer()
                        .RegisterType<OpenGenericInjectionMethod>(
                        new InjectionMethod("InjectMe"));
                });
        }

        [TestMethod]
        public void CannotConfigureMethodWithOutParams()
        {
            AssertExtensions.AssertException<InvalidOperationException>(() =>
                {
                    new UnityContainer().RegisterType<OutParams>(
                        new InjectionMethod("InjectMe", 12));
                });
        }

        [TestMethod]
        public void CannotConfigureMethodWithRefParams()
        {
            AssertExtensions.AssertException<InvalidOperationException>(() =>
                {
                    new UnityContainer()
                        .RegisterType<RefParams>(
                        new InjectionMethod("InjectMe", 15));
                });
        }

        [TestMethod]
        public void CanInvokeInheritedMethod()
        {
            IUnityContainer container = new UnityContainer()
                          .RegisterType<InheritedClass>(
                                  new InjectionMethod("InjectMe"));

            InheritedClass result = container.Resolve<InheritedClass>();
            Assert.IsTrue(result.WasInjected);
        }

        public class LegalInjectionMethod
        {
            public bool WasInjected = false;

            public void InjectMe()
            {
                WasInjected = true;
            }
        }

        public class OpenGenericInjectionMethod
        {
            public void InjectMe<T>()
            {
            }
        }

        public class OutParams
        {
            public void InjectMe(out int x)
            {
                x = 2;
            }
        }

        public class RefParams
        {
            public void InjectMe(ref int x)
            {
                x *= 2;
            }
        }

        public class InheritedClass : LegalInjectionMethod
        {
        }
    }
}
