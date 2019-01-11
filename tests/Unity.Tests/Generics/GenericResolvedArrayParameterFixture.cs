using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using Unity.Injection;

namespace Unity.Tests.v5.Generics
{
    [TestClass]
    public class GenericResolvedArrayParameterFixture
    {
        [TestMethod]
        public void MatchesArrayOfGenericTypeOnly()
        {
            var parameterValue = (IEquatable<Type>)new GenericResolvedArrayParameter("T");

            Type genericTypeT
                = this.GetType().GetTypeInfo().GetDeclaredMethod("GetT")
                    .GetGenericArguments()[0];
            Type genericTypeU
                = this.GetType().GetTypeInfo().GetDeclaredMethod("GetU")
                    .GetGenericArguments()[0];

            Assert.IsFalse(parameterValue.Equals(genericTypeT));
            Assert.IsFalse(parameterValue.Equals(genericTypeU));
            Assert.IsFalse(parameterValue.Equals(typeof(object)));
            Assert.IsTrue( parameterValue.Equals(genericTypeT.MakeArrayType(1)));
            Assert.IsFalse(parameterValue.Equals(genericTypeT.MakeArrayType(2)));
            Assert.IsFalse(parameterValue.Equals(genericTypeU.MakeArrayType(1)));
            Assert.IsFalse(parameterValue.Equals(typeof(object[])));
        }

        [TestMethod]
        public void CanCallConstructorTakingGenericParameterArray()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(
                    typeof(ClassWithOneArrayGenericParameter<>),
                    new InjectionConstructor(new GenericParameter("T[]")));

            Account a0 = new Account();
            container.RegisterInstance<Account>("a0", a0);
            Account a1 = new Account();
            container.RegisterInstance<Account>("a1", a1);
            Account a2 = new Account();
            container.RegisterInstance<Account>(a2);

            ClassWithOneArrayGenericParameter<Account> result
                = container.Resolve<ClassWithOneArrayGenericParameter<Account>>();
            Assert.IsFalse(result.DefaultConstructorCalled);
            Assert.AreEqual(2, result.InjectedValue.Length);
            Assert.AreSame(a0, result.InjectedValue[0]);
            Assert.AreSame(a1, result.InjectedValue[1]);
        }

        [TestMethod]
        public void CanCallConstructorTakingGenericParameterArrayWithValues()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(
                    typeof(ClassWithOneArrayGenericParameter<>),
                    new InjectionConstructor(
                        new GenericResolvedArrayParameter(
                            "T",
                            new GenericParameter("T", "a2"),
                            new GenericParameter("T", "a1"))));

            Account a0 = new Account();
            container.RegisterInstance<Account>("a0", a0);
            Account a1 = new Account();
            container.RegisterInstance<Account>("a1", a1);
            Account a2 = new Account();
            container.RegisterInstance<Account>("a2", a2);

            ClassWithOneArrayGenericParameter<Account> result
                = container.Resolve<ClassWithOneArrayGenericParameter<Account>>();
            Assert.IsFalse(result.DefaultConstructorCalled);
            Assert.AreEqual(2, result.InjectedValue.Length);
            Assert.AreSame(a2, result.InjectedValue[0]);
            Assert.AreSame(a1, result.InjectedValue[1]);
        }

        [TestMethod]
        public void CanSetPropertyWithGenericParameterArrayType()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ClassWithOneArrayGenericParameter<>),
                                       new InjectionConstructor(),
                                       new InjectionProperty("InjectedValue", new GenericParameter("T()")));

            Account a0 = new Account();
            container.RegisterInstance<Account>("a1", a0);
            Account a1 = new Account();
            container.RegisterInstance<Account>("a2", a1);
            Account a2 = new Account();
            container.RegisterInstance<Account>(a2);

            ClassWithOneArrayGenericParameter<Account> result
                = container.Resolve<ClassWithOneArrayGenericParameter<Account>>();
            Assert.IsTrue(result.DefaultConstructorCalled);
            Assert.AreEqual(2, result.InjectedValue.Length);
            Assert.AreSame(a0, result.InjectedValue[0]);
            Assert.AreSame(a1, result.InjectedValue[1]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AppropriateExceptionIsThrownWhenNoMatchingConstructorCanBeFound()
        {
            new UnityContainer()
                .RegisterType(typeof(ClassWithOneGenericParameter<>),
                    new InjectionConstructor(new GenericResolvedArrayParameter("T")));
        }

        private void GetT<T>() { }
        private void GetU<U>() { }

        public class ClassWithOneArrayGenericParameter<T>
        {
            private T[] injectedValue;
            public readonly bool DefaultConstructorCalled;

            public ClassWithOneArrayGenericParameter()
            {
                DefaultConstructorCalled = true;
            }

            public ClassWithOneArrayGenericParameter(T[] injectedValue)
            {
                DefaultConstructorCalled = false;

                this.injectedValue = injectedValue;
            }

            public T[] InjectedValue
            {
                get { return this.injectedValue; }
                set { this.injectedValue = value; }
            }
        }

        public class ClassWithOneGenericParameter<T>
        {
        }
    }
}
