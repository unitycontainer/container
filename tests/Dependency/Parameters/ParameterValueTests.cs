using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity.Injection;

namespace Injection.Parameters
{
    [TestClass]
    public class ParameterValueTests
    {

        [DataTestMethod]
        [DynamicData(nameof(SupportedParameters))]
        public void ToStringTest(ParameterValue parameter)
        {
            var name = parameter.GetType().Name;

            Assert.IsTrue(parameter.ToString().StartsWith(name));
        }


        #region Test Data

        public static IEnumerable<object[]> SupportedParameters
        {
            get
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
        }

        #endregion
    }
}
