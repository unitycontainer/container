using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity.Injection;

namespace Injection.Parameters
{
    [TestClass]
    public class ParameterValidationTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InjectionParameterCtorTest()
        {
            new InjectionParameter(null);
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
    }
}
