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

        public static IEnumerable<object[]> SupportedParametersData()
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

        #endregion
    }
}
