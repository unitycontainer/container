using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Injection;

namespace Injection.Parameters
{
    [TestClass]
    public class EquatabilityTests
    {
        #region Fields
        
        public void TestMethod<T, V>(T first, V second) => throw new NotImplementedException();

        private static Type TrueType =
            typeof(EquatabilityTests).GetMethod(nameof(TestMethod))
                          .GetParameters()
                          .First()
                          .ParameterType;

        private static Type FalseType =
            typeof(EquatabilityTests).GetMethod(nameof(TestMethod))
                          .GetParameters()
                          .Last()
                          .ParameterType;

        private static Type TrueArrayType = TrueType.MakeArrayType();
        private static Type FalseArrayType = FalseType.MakeArrayType();

        #endregion


        #region IEquatable

        [Ignore]
        [DataTestMethod]
        [DynamicData(nameof(GetEqualsAnyTypeData), DynamicDataSourceType.Method)]
        public virtual void EqualsAnyTypeTest(ParameterValue parameter)
        {
            // Validate
            //Assert.IsTrue(parameter.Matching(typeof(int)));
            //Assert.IsTrue(parameter.Matching(typeof(object)));
            //Assert.IsTrue(parameter.Matching(typeof(string)));
            //Assert.IsTrue(parameter.Matching(typeof(List<>)));
            //Assert.IsTrue(parameter.Matching(typeof(List<string>)));
            //Assert.IsTrue(parameter.Matching(typeof(string[])));
            //Assert.IsTrue(parameter.Matching(typeof(object[])));
            //Assert.IsTrue(parameter.Matching(typeof(int[])));
        }

        [Ignore]
        [DataTestMethod]
        [DynamicData(nameof(GetEqualsValueTypeData), DynamicDataSourceType.Method)]
        public virtual void EqualsValueTypeTest(ParameterValue parameter)
        {
            // Validate
            //Assert.IsTrue(parameter.Matching(typeof(int)));
            //Assert.IsTrue(parameter.Matching(typeof(object)));
            //Assert.IsFalse(parameter.Matching(typeof(string)));
        }

        [Ignore]
        [DataTestMethod]
        [DynamicData(nameof(GetEqualsArrayTypeData), DynamicDataSourceType.Method)]
        public virtual void EqualsArrayTypeTest(ParameterValue parameter)
        {
            // Validate
            //Assert.IsTrue(parameter.Matching(typeof(string[])));
            //Assert.IsTrue(parameter.Matching(typeof(object[])));
            //Assert.IsFalse(parameter.Matching(typeof(string[,])));
            //Assert.IsFalse(parameter.Matching(typeof(int)));
            //Assert.IsFalse(parameter.Matching(typeof(int[])));
        }

        [Ignore]
        [DataTestMethod]
        [DynamicData(nameof(GetEqualsGenericTypeData), DynamicDataSourceType.Method)]
        public virtual void EqualsGenericTypeTest(ParameterValue parameter)
        {
            // Validate
            //Assert.IsTrue(parameter.Matching(typeof(List<>)));
            //Assert.IsTrue(parameter.Matching(typeof(List<string>)));
            //Assert.IsFalse(parameter.Matching(typeof(IEnumerable<>)));
        }

        [Ignore]
        [DataTestMethod]
        [DynamicData(nameof(GetEqualsGenericParameterData), DynamicDataSourceType.Method)]
        public virtual void EqualsGenericTest(ParameterValue parameter)
        {
            // Validate
            //Assert.IsTrue(parameter.Matching(TrueType));
            //Assert.IsFalse(parameter.Matching(typeof(string)));
            //Assert.IsFalse(parameter.Matching(FalseType));
        }

        [Ignore]
        [DataTestMethod]
        [DynamicData(nameof(GetEqualsGenericArrayParameterData), DynamicDataSourceType.Method)]
        public virtual void EqualsGenericArrayTest(ParameterValue parameter)
        {
            // Validate
            //Assert.IsTrue(parameter.Matching(TrueArrayType));
            //Assert.IsFalse(parameter.Matching(typeof(string)));
            //Assert.IsFalse(parameter.Matching(typeof(string[])));
            //Assert.IsFalse(parameter.Matching(typeof(string[,])));
            //Assert.IsFalse(parameter.Matching(FalseArrayType));
        }

        #endregion


        #region Test Data

        public static IEnumerable<object[]> GetEqualsAnyTypeData()
        {
            yield return new object[] { new OptionalParameter(), };
            yield return new object[] { new OptionalParameter(string.Empty) };

            yield return new object[] { new ResolvedParameter() };
            yield return new object[] { new ResolvedParameter(string.Empty) };
        }

        public static IEnumerable<object[]> GetEqualsValueTypeData()
        {
            yield return new object[] { new InjectionParameter(0) };
            yield return new object[] { new InjectionParameter(typeof(int), 0) };
            yield return new object[] { new InjectionParameter<int>(0) };

            yield return new object[] { new OptionalParameter(typeof(int)) };
            yield return new object[] { new OptionalParameter<int>() };
            yield return new object[] { new OptionalParameter(typeof(int), string.Empty) };
            yield return new object[] { new OptionalParameter<int>(string.Empty) };

            yield return new object[] { new ResolvedParameter(typeof(int)) };
            yield return new object[] { new ResolvedParameter<int>() };
            yield return new object[] { new ResolvedParameter(typeof(int), string.Empty) };
            yield return new object[] { new ResolvedParameter<int>(string.Empty) };
        }

        public static IEnumerable<object[]> GetEqualsArrayTypeData()
        {
            yield return new object[] { new OptionalParameter(typeof(string[])) };
            yield return new object[] { new OptionalParameter<string[]>() };
            yield return new object[] { new OptionalParameter(typeof(string[]), string.Empty) };
            yield return new object[] { new OptionalParameter<string[]>(string.Empty) };

            yield return new object[] { new ResolvedParameter(typeof(string[])) };
            yield return new object[] { new ResolvedParameter<string[]>() };
            yield return new object[] { new ResolvedParameter(typeof(string[]), string.Empty) };
            yield return new object[] { new ResolvedParameter<string[]>(string.Empty) };

            yield return new object[] { new ResolvedArrayParameter(typeof(string)) };
            yield return new object[] { new ResolvedArrayParameter<string>() };
            yield return new object[] { new ResolvedArrayParameter(typeof(string), "string") };
            yield return new object[] { new ResolvedArrayParameter(typeof(string), typeof(string), "string") };

            yield return new object[] { new InjectionParameter(new string[0]) };
            yield return new object[] { new InjectionParameter(typeof(string[]), new string[0]) };
            yield return new object[] { new InjectionParameter<string[]>(new string[0]) };
        }

        public static IEnumerable<object[]> GetEqualsGenericTypeData()
        {
            yield return new object[] { new InjectionParameter(typeof(List<>), 0) };

            yield return new object[] { new OptionalParameter(typeof(List<>)) };
            yield return new object[] { new OptionalParameter(typeof(List<>), string.Empty) };

            yield return new object[] { new ResolvedParameter(typeof(List<>)) };
            yield return new object[] { new ResolvedParameter(typeof(List<>), string.Empty) };
        }

        public static IEnumerable<object[]> GetEqualsGenericParameterData()
        {
            yield return new object[] { new GenericParameter("T") };
            yield return new object[] { new GenericParameter("T", string.Empty) };

            yield return new object[] { new OptionalGenericParameter("T") };
            yield return new object[] { new OptionalGenericParameter("T", string.Empty) };
        }

        public static IEnumerable<object[]> GetEqualsGenericArrayParameterData()
        {
            yield return new object[] { new GenericResolvedArrayParameter("T") };
            yield return new object[] { new GenericResolvedArrayParameter("T", string.Empty) };

            yield return new object[] { new OptionalGenericParameter("T[]") };
            yield return new object[] { new OptionalGenericParameter("T[]", string.Empty) };

            yield return new object[] { new GenericParameter("T[]") };
            yield return new object[] { new GenericParameter("T[]", string.Empty) };
        }

        #endregion
    }
}
