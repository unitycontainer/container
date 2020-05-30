using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Resolution;
using static Injection.Parameters.ResolutionTests;

namespace Injection.Parameters
{
    [TestClass]
    public class ValidationTests
    {
        #region Fields

        private const string DefaultValue = "default";

        public void TestMethod(string first, string last = DefaultValue) => throw new NotImplementedException();

        private static ParameterInfo NoDefaultInfo =
            typeof(ValidationTests).GetMethod(nameof(TestMethod))
                                   .GetParameters()
                                   .First();

        private static ParameterInfo DefaultInfo =
            typeof(ValidationTests).GetMethod(nameof(TestMethod))
                                   .GetParameters()
                                   .Last();

        #endregion

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InjectionParameterCtorTest()
        {
            new InjectionParameter(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GenericParameterCtorTest()
        {
            new GenericParameter(null);
        }


        // Issue https://github.com/unitycontainer/abstractions/issues/146
        [Ignore]
        [TestMethod]
        public void ResolvedArrayParameterCtorTest()
        {
            new ResolvedArrayParameter(null, typeof(string));
        }

        [Ignore]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolvedArrayParameterElementTest()
        {
            new ResolvedArrayParameter(typeof(string), null);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetSupportedParametersData), DynamicDataSourceType.Method)]
        public void ToStringTest(ParameterValue parameter)
        {
            var name = parameter.GetType().Name;

            Assert.IsTrue(parameter.ToString().StartsWith(name));
        }

        [Ignore] // TODO: validate
        [DataTestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [DynamicData(nameof(GetSupportedParametersData), DynamicDataSourceType.Method)]
        public void EqualsValidationTest(ParameterValue parameter)
        {
            Assert.IsTrue(parameter.Equals(null));
        }


        #region Exceptions

        [DataTestMethod]
        [DynamicData(nameof(GetOptionalParametersData), DynamicDataSourceType.Method)]
        public void OptionalExceptionTest(IResolverFactory<Type> factory)
        {
            var context = new DictionaryContext() as IResolveContext;
            var resolver = factory.GetResolver<IResolveContext>(typeof(string));

            // Validate
            Assert.IsNotNull(resolver);

            Assert.IsNull(resolver(ref context));
        }

        [DataTestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        [DynamicData(nameof(GetOptionalParametersData), DynamicDataSourceType.Method)]
        public void OptionalCircularExceptionTest(IResolverFactory<Type> factory)
        {
            var context = new CircularExceptionContect() as IResolveContext;
            var resolver = factory.GetResolver<IResolveContext>(typeof(string));

            // Validate
            Assert.IsNotNull(resolver);

            _ = resolver(ref context);
        }


        [DataTestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        [DynamicData(nameof(GetOptionalParametersData), DynamicDataSourceType.Method)]
        public void OptionalCircularExceptionInfoTest(IResolverFactory<ParameterInfo> factory)
        {
            var context = new CircularExceptionContect() as IResolveContext;
            var resolver = factory.GetResolver<IResolveContext>(NoDefaultInfo);

            // Validate
            Assert.IsNotNull(resolver);

            _ = resolver(ref context);
        }

        #endregion


        #region Default Value

        [DataTestMethod]
        [DynamicData(nameof(Getissues147Data), DynamicDataSourceType.Method)]
        // TODO: issues 147: [DynamicData(nameof(GetOptionalParametersData), DynamicDataSourceType.Method)]
        public void OptionalDefaultTest(IResolverFactory<ParameterInfo> factory)
        {
            var context = new DictionaryContext() as IResolveContext;
            var resolver = factory.GetResolver<IResolveContext>(DefaultInfo);

            // Validate
            Assert.IsNotNull(resolver);

            var value = resolver(ref context);
            Assert.AreEqual(DefaultValue, value);
        }

        [DataTestMethod]
        [DynamicData(nameof(Getissues147Data), DynamicDataSourceType.Method)]
        // TODO: issues 147: [DynamicData(nameof(GetOptionalParametersData), DynamicDataSourceType.Method)]
        public void OptionalNoDefaultTest(IResolverFactory<ParameterInfo> factory)
        {
            var context = new DictionaryContext() as IResolveContext;
            var resolver = factory.GetResolver<IResolveContext>(NoDefaultInfo);

            // Validate
            Assert.IsNotNull(resolver);

            Assert.IsNull(resolver(ref context));
        }

        // TODO: issues 147: 
        public static IEnumerable<object[]> Getissues147Data()
        {
            yield return new object[] { new OptionalParameter() };
            yield return new object[] { new OptionalParameter(typeof(string)) };
            yield return new object[] { new OptionalParameter(string.Empty) };
            yield return new object[] { new OptionalParameter(typeof(string), string.Empty) };
        }

        #endregion


        #region Test Data

        public static IEnumerable<object[]> GetSupportedParametersData()
        {
            yield return new object[] { new InjectionParameter(string.Empty) };
            yield return new object[] { new InjectionParameter(typeof(string), null) };
            yield return new object[] { new OptionalParameter() };
            yield return new object[] { new ResolvedParameter() };
            yield return new object[] { new ResolvedArrayParameter(typeof(string)) };
            yield return new object[] { new GenericParameter("T[]") };
            yield return new object[] { new OptionalGenericParameter("T") };
            yield return new object[] { new OptionalGenericParameter("T", string.Empty) };
            yield return new object[] { new GenericResolvedArrayParameter("T[]") };
        }

        public static IEnumerable<object[]> GetOptionalParametersData()
        {
            yield return new object[] { new OptionalGenericParameter("T") };
            yield return new object[] { new OptionalGenericParameter("T", string.Empty) };

            yield return new object[] { new OptionalParameter() };
            yield return new object[] { new OptionalParameter(typeof(string)) };
            yield return new object[] { new OptionalParameter(string.Empty) };
            yield return new object[] { new OptionalParameter(typeof(string), string.Empty) };
        }


        public class CircularExceptionContect : IResolveContext
        {
            public IUnityContainer Container => throw new NotImplementedException();

            public Type Type => throw new NotImplementedException();

            public string Name => throw new NotImplementedException();

            public void Clear(Type type, string name, Type policyInterface) => throw new NotImplementedException();

            public object Get(Type type, Type policyInterface) => throw new NotImplementedException();

            public object Get(Type type, string name, Type policyInterface) => throw new NotImplementedException();

            public object Resolve(Type type, string name)
            {
                throw new CircularDependencyException(type, name);
            }

            public void Set(Type type, Type policyInterface, object policy) => throw new NotImplementedException();

            public void Set(Type type, string name, Type policyInterface, object policy) => throw new NotImplementedException();
        }

        #endregion
    }
}
