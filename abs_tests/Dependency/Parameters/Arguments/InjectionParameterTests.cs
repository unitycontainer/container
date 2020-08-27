using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity.Injection;

namespace Injection.Parameters
{
    [TestClass]
    public class InjectionParameterTests
    {
        [TestMethod]
        public void ConstructorTest()
        {
            // Validate can initialize with null and type
            Assert.IsNotNull(new InjectionParameter<string>(null));
            Assert.IsNotNull(new InjectionParameter(typeof(string), null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidationTest()
        {
            // Validate throws on no type
            new InjectionParameter(null);
        }
    }
}
