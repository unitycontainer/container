using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Injection;
using Unity.Resolution;
using static Injection.Parameters.ResolutionTests;

namespace Injection.Parameters
{
    [TestClass]
    public class ValidationTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InjectionParameterCtorTest()
        {
            // Validate can initialize with null and type
            Assert.IsNotNull(new InjectionParameter<string>(null));
            Assert.IsNotNull(new InjectionParameter(typeof(string), null));

            // Validate throws on no type
            new InjectionParameter(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GenericParameterCtorTest()
        {
            // Validate throws on null
            new GenericParameter(null);
        }

        [DataTestMethod]
        [DynamicData(nameof(SupportedParametersData), DynamicDataSourceType.Method)]
        public void ToStringTest(ParameterValue parameter)
        {
            var name = parameter.GetType().Name;

            Assert.IsTrue(parameter.ToString().StartsWith(name));
        }

        [DataTestMethod]
        [DynamicData(nameof(ParametersWithTypeData), DynamicDataSourceType.Method)]
        public void EqualsValidationTest(ParameterValue parameter)
        {
            Assert.IsFalse(parameter.Equals(null));
        }


        #region ResolvedArrayParameter

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArrayParameterNullTest()
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

        #endregion


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
        public static IEnumerable<object[]> SupportedParametersData()
        {
            yield return new object[] { new InjectionParameter(string.Empty) };
            yield return new object[] { new InjectionParameter(typeof(string), null) };
            yield return new object[] { new OptionalParameter() };
            yield return new object[] { new ResolvedParameter() };
            yield return new object[] { new ResolvedParameter(string.Empty) };
            yield return new object[] { new ResolvedArrayParameter(typeof(string)) };
            yield return new object[] { new GenericParameter("T[]") };
            yield return new object[] { new OptionalGenericParameter("T") };
            yield return new object[] { new OptionalGenericParameter("T", string.Empty) };
            yield return new object[] { new GenericResolvedArrayParameter("T[]") };
        }

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

        #endregion
    }
}
