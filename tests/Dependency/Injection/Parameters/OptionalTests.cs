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
    public class OptionalTests
    {
        #region Fields

        private const string TestValue = "test";

        #endregion


        [DataTestMethod]
        [DynamicData(nameof(OptionalParametersData), DynamicDataSourceType.Method)]
        public void OptionalParametersTest(IResolverFactory<Type> factory)
        {
            var context = new DictionaryContext() as IResolveContext;
            var resolver = factory.GetResolver<IResolveContext>(typeof(string));

            // Validate
            Assert.IsNotNull(resolver);

            Assert.IsNull(resolver(ref context));
        }

        [DataTestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        [DynamicData(nameof(OptionalParametersData), DynamicDataSourceType.Method)]
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
        [DynamicData(nameof(OptionalParametersData), DynamicDataSourceType.Method)]
        public void OptionalCircularExceptionInfoTest(IResolverFactory<ParameterInfo> factory)
        {
            var context = new CircularExceptionContect() as IResolveContext;
            var resolver = factory.GetResolver<IResolveContext>(NoDefaultInfo);

            // Validate
            Assert.IsNotNull(resolver);

            _ = resolver(ref context);
        }


        #region Default Value

        [DataTestMethod]
        [DynamicData(nameof(OptionalParametersData), DynamicDataSourceType.Method)]
        public void OptionalResolvedTest(IResolverFactory<ParameterInfo> factory)
        {
            var context = new DictionaryContext() { { typeof(string), TestValue } } as IResolveContext;
            var resolver = factory.GetResolver<IResolveContext>(DefaultInfo);

            // Validate
            Assert.IsNotNull(resolver);

            var value = resolver(ref context);
            Assert.AreSame(TestValue, value);
        }

        [DataTestMethod]
        [DynamicData(nameof(OptionalParametersData), DynamicDataSourceType.Method)]
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
        [DynamicData(nameof(OptionalParametersData), DynamicDataSourceType.Method)]
        public void OptionalNoDefaultTest(IResolverFactory<ParameterInfo> factory)
        {
            var context = new DictionaryContext() as IResolveContext;
            var resolver = factory.GetResolver<IResolveContext>(NoDefaultInfo);

            // Validate
            Assert.IsNotNull(resolver);

            Assert.IsNull(resolver(ref context));
        }


        #endregion


        #region Test Data

        public static IEnumerable<object[]> OptionalParametersData()
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
