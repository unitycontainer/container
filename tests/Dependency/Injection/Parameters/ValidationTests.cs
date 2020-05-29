using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity.Injection;

namespace Injection.Parameters
{
    [TestClass]
    public class ValidationTests
    {
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

        #endregion
    }
}
