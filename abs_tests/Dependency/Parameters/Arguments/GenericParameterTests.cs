using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity.Injection;

namespace Injection.Parameters
{
    [TestClass]
    public class GenericParameterTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidationTest()
        {
            // Validate throws on null
            new GenericParameter(null);
        }

    }
}
