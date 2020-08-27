using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;

namespace Exceptions.Tests
{
    [TestClass]
    public partial class ExceptionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidationTest()
        {
            _ = new ResolutionFailedException(null, null, null);
        }
    }
}
