using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity.Injection;

namespace Injection.Parameters
{
    [TestClass]
    public class ParameterBaseTests
    {
        [TestMethod]
        public virtual void ParameterTypeTest()
        {
        }


        #region IEquatable


        [DataTestMethod]
        [DynamicData(nameof(ParameterBaseData.GetEqualsAnyTypeData), typeof(ParameterBaseData), DynamicDataSourceType.Method)]
        public virtual void EqualsAnyTypeTest(ParameterValue parameter)
        {
            // Validate
            Assert.IsTrue(parameter.Equals(typeof(int)));
            Assert.IsTrue(parameter.Equals(typeof(object)));
            Assert.IsTrue(parameter.Equals(typeof(string)));
            Assert.IsTrue(parameter.Equals(typeof(List<>)));
            Assert.IsTrue(parameter.Equals(typeof(List<string>)));
            Assert.IsTrue(parameter.Equals(typeof(string[])));
            Assert.IsTrue(parameter.Equals(typeof(object[])));
            Assert.IsTrue(parameter.Equals(typeof(int[])));
        }

        [DataTestMethod]
        [DynamicData(nameof(ParameterBaseData.GetEqualsValueTypeData), typeof(ParameterBaseData), DynamicDataSourceType.Method)]
        public virtual void EqualsValueTypeTest(ParameterValue parameter)
        {
            // Validate
            Assert.IsTrue(parameter.Equals(typeof(int)));
            Assert.IsTrue(parameter.Equals(typeof(object)));
            Assert.IsFalse(parameter.Equals(typeof(string)));
        }

        [DataTestMethod]
        [DynamicData(nameof(ParameterBaseData.GetEqualsArrayTypeData), typeof(ParameterBaseData), DynamicDataSourceType.Method)]
        public virtual void EqualsArrayTypeTest(ParameterValue parameter)
        {
            // Validate
            Assert.IsTrue(parameter.Equals(typeof(string[])));
            Assert.IsTrue(parameter.Equals(typeof(object[])));
            Assert.IsFalse(parameter.Equals(typeof(int)));
            Assert.IsFalse(parameter.Equals(typeof(int[])));
        }

        [DataTestMethod]
        [DynamicData(nameof(ParameterBaseData.GetEqualsGenericTypeData), typeof(ParameterBaseData), DynamicDataSourceType.Method)]
        public virtual void EqualsGenericTypeTest(ParameterValue parameter)
        {
            // Validate
            Assert.IsTrue(parameter.Equals(typeof(List<>)));
            Assert.IsFalse(parameter.Equals(typeof(IEnumerable<>)));
            Assert.IsFalse(parameter.Equals(typeof(List<string>)));
        }

        #endregion


        #region Test Data


        #endregion
    }
}
