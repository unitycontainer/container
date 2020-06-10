using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using Unity;

namespace Exceptions.Tests
{
    public partial class ExceptionTests
    {
        [DataTestMethod]
        [DynamicData(nameof(ExceptionTestData), DynamicDataSourceType.Method)]
        public void BinaryFormatterTest(ResolutionFailedException ex)
        {
            // Arrange
            IFormatter formatter = new BinaryFormatter();

            // Act
            var inner = ex.InnerException;
            var exception = ReFormatException(formatter, ex);

            // Validate
            Assert.IsNotNull(exception);
            Assert.AreEqual(ex.TypeRequested, exception.TypeRequested);
            Assert.AreEqual(ex.NameRequested, exception.NameRequested);
            Assert.AreEqual(ex.Message, exception.Message);
        }

        [DataTestMethod]
        [DynamicData(nameof(ExceptionTestData), DynamicDataSourceType.Method)]
        public void SoapFormatterTest(ResolutionFailedException ex)
        {
            // Arrange
            IFormatter formatter = new SoapFormatter();

            // Act
            var exception = ReFormatException(formatter, ex);

            // Validate
            Assert.IsNotNull(exception);
            Assert.AreEqual(ex.TypeRequested, exception.TypeRequested);
            Assert.AreEqual(ex.NameRequested, exception.NameRequested);
            Assert.AreEqual(ex.Message, exception.Message);
        }

        #region Test Data

        public static IEnumerable<object[]> ExceptionTestData()
        {
            yield return new object[] { new ResolutionFailedException(typeof(ExceptionTests), null, null) };
            yield return new object[] { new ResolutionFailedException(typeof(ExceptionTests), string.Empty, null) };
            yield return new object[] { new ResolutionFailedException(typeof(ExceptionTests), "name", "message") };
        }


        #endregion


        #region Implementation

        private ResolutionFailedException ReFormatException(IFormatter formatter, ResolutionFailedException exception)
        { 
            using var stream = new MemoryStream();
            
            formatter.Serialize(stream, exception);
            stream.Seek(0, SeekOrigin.Begin);

            return (ResolutionFailedException)formatter.Deserialize(stream);
        }

        #endregion
    }
}
