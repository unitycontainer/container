using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Injection;
using Unity.Resolution;

namespace Injection.Parameters
{
    [TestClass]
    public class ArrayParameterTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArrayParameter_Null()
        {
            new ResolvedArrayParameter(null);
        }

        [DataTestMethod]
        [DynamicData(nameof(ResolvedArrayParameterData), DynamicDataSourceType.Method)]
        public void ArrayParameterCtorTest(Type elementType, object[] elementValues)
        {
            Assert.IsNotNull(new ResolvedArrayParameter(elementType, elementValues));
        }

        [DataTestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [DynamicData(nameof(ResolvedArrayInvalidData), DynamicDataSourceType.Method)]
        public void ArrayParameterInvalid(Type elementType, object[] elementValues)
        {
            Assert.IsNotNull(new ResolvedArrayParameter(elementType, elementValues));
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GenericArrayTest()
        {
            // Arrange
            var factory = new GenericResolvedArrayParameter("T[]");

            ParameterInfo info =
                typeof(TestClass<string>).GetMethod(nameof(TestClass<string>.TestMethod))
                                           .GetParameters()
                                           .First();
            // Act
            _ = factory.GetResolver<IResolveContext>(info);
        }


        #region Test Data

        public static IEnumerable<object[]> ParametersWithTypeData()
        {
            yield return new object[] { new InjectionParameter(string.Empty) };
            yield return new object[] { new InjectionParameter(typeof(string), null) };
            yield return new object[] { new OptionalParameter(typeof(string)) };
            yield return new object[] { new ResolvedParameter(typeof(string)) };
            yield return new object[] { new ResolvedArrayParameter(typeof(string)) };
            yield return new object[] { new GenericParameter("T[]") };
            yield return new object[] { new OptionalGenericParameter("T") };
            yield return new object[] { new OptionalGenericParameter("T", string.Empty) };
            yield return new object[] { new GenericResolvedArrayParameter("T[]") };
        }

        public static IEnumerable<object[]> ResolvedArrayParameterData()
        {
            yield return new object[] { typeof(object), null };
            yield return new object[] { typeof(object), new object[] { } };
            yield return new object[] { typeof(object), new object[] { new InjectionParameter(typeof(object), null) } };
            yield return new object[] { typeof(object), new object[] { typeof(object) } };
            yield return new object[] { typeof(IList<string>), new object[] { new List<string>() } };
        }

        public static IEnumerable<object[]> ResolvedArrayInvalidData()
        {
            yield return new object[] { typeof(IList<string>), new object[] { typeof(List<string>) } };
            yield return new object[] { typeof(object), new object[] { null } };
        }


        public const string DefaultValue = "default";

        public class TestClass<T>
        {
            public void TestMethod(T array, string name = DefaultValue) => throw new NotImplementedException();
        }

        #endregion
    }
}
