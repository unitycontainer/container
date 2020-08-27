using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reflection;
using Unity;
using Unity.Abstractions.Tests;
using Unity.Exceptions;
using Unity.Injection;

namespace Dependency.Annotation
{
    [TestClass]
    public class AttributesTests
    {
        [DataTestMethod]
        [DynamicData(nameof(DependencyResolutionAttributes))]
        public void DependencyAttributesTest(DependencyResolutionAttribute attribute)
        {
            Assert.IsNull(attribute.Name);
            Assert.AreEqual(MatchRank.ExactMatch, attribute.MatchTo(typeof(AttributesTests)));
        }

        [DataTestMethod]
        [DynamicData(nameof(NamedAttributes))]
        public void NamedAttributesTest(DependencyResolutionAttribute attribute)
        {
            Assert.IsNotNull(attribute.Name);
            Assert.AreEqual(MatchRank.ExactMatch, attribute.MatchTo(typeof(AttributesTests)));
        }

        [TestMethod]
        public void FactoryPass()
        {
            // Arrange
            var context  = new DictionaryContext() { { typeof(string), TestValue } };
            var field    = typeof(AttributesTests).GetField(nameof(TestField));
            var property = typeof(AttributesTests).GetProperty(nameof(TestProperty));

            // Act
            var required = new DependencyAttribute();
            var optional = new OptionalDependencyAttribute();

            // Type
            var resReq = required.GetResolver<DictionaryContext>(typeof(string));
            var resOpt = optional.GetResolver<DictionaryContext>(typeof(string));
            // Field
            var resReqField = required.GetResolver<DictionaryContext>(field);
            var resOptField = optional.GetResolver<DictionaryContext>(field);
            // Property
            var resReqProperty = required.GetResolver<DictionaryContext>(property);
            var resOptProperty = optional.GetResolver<DictionaryContext>(property);

            // Validate
            Assert.IsNotNull(resReq);
            Assert.IsNotNull(resOpt);
            Assert.IsNotNull(resReqField);
            Assert.IsNotNull(resOptField);
            Assert.IsNotNull(resReqProperty);
            Assert.IsNotNull(resOptProperty);

            Assert.AreSame(TestValue, resReq(ref context));
            Assert.AreSame(TestValue, resOpt(ref context));
            Assert.AreSame(TestField, resReqField(ref context));
            Assert.AreSame(TestField, resOptField(ref context));
            Assert.AreSame(TestProperty, resReqProperty(ref context));
            Assert.AreSame(TestProperty, resOptProperty(ref context));
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void FactoryRequired()
        {
            // Arrange
            var context = new DictionaryContext();

            // Act
            var required = new DependencyAttribute();

            var resReq = required.GetResolver<DictionaryContext>(typeof(string));

            // Validate
            Assert.IsNotNull(resReq);

            _ = resReq(ref context);
        }

        [TestMethod]
        public void FactoryOptional()
        {
            // Arrange
            var context = new DictionaryContext();

            // Act
            var optional = new OptionalDependencyAttribute();
            var resOpt = optional.GetResolver<DictionaryContext>(typeof(string));

            // Validate
            Assert.IsNotNull(resOpt);
            Assert.AreSame(null, resOpt(ref context));
        }

        [TestMethod]
        public void ParameterFactoryPass()
        {
            // Arrange
            var context = new DictionaryContext() { { typeof(string), TestValue } };
            var required = new DependencyAttribute();
            var optional = new OptionalDependencyAttribute();
            var first = Parameters[0];
            var second = Parameters[1];

            // Act
            var resReqFirst = required.GetResolver<DictionaryContext>(first);
            var resOptFirst = optional.GetResolver<DictionaryContext>(first);
            var resReqSecond = required.GetResolver<DictionaryContext>(second);
            var resOptSecond = optional.GetResolver<DictionaryContext>(second);

            // Validate
            Assert.IsNotNull(resReqFirst);
            Assert.IsNotNull(resOptFirst );
            Assert.IsNotNull(resReqSecond);
            Assert.IsNotNull(resOptSecond);

            Assert.AreSame(TestValue, resReqFirst(ref context));
            Assert.AreSame(TestValue, resOptFirst(ref context));
            Assert.AreSame(TestValue, resReqSecond(ref context));
            Assert.AreSame(TestValue, resOptSecond(ref context));
        }

        [TestMethod]
        public void ParameterFactoryDefault()
        {
            // Arrange
            var context = new DictionaryContext();
            var required = new DependencyAttribute();
            var optional = new OptionalDependencyAttribute();
            var info = Parameters[1];

            // Act
            var resReqSecond = required.GetResolver<DictionaryContext>(info);
            var resOptSecond = optional.GetResolver<DictionaryContext>(info);

            // Validate
            Assert.IsNotNull(resReqSecond);
            Assert.IsNotNull(resOptSecond);

            Assert.IsNull(resReqSecond(ref context));
            Assert.IsNull(resOptSecond(ref context));
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void ParameterFactoryNoDefault()
        {
            // Arrange
            var context = new DictionaryContext();
            var required = new DependencyAttribute();

            // Act
            var resReqSecond = required.GetResolver<DictionaryContext>(Parameters[0]);

            // Validate
            Assert.IsNotNull(resReqSecond);

            _ =resReqSecond(ref context);
        }

        [TestMethod]
        public void ParameterFactoryOptional()
        {
            // Arrange
            var context = new DictionaryContext();
            var optional = new OptionalDependencyAttribute();

            // Act
            var resolver = optional.GetResolver<DictionaryContext>(Parameters[0]);

            // Validate
            Assert.IsNotNull(resolver);

            Assert.IsNull(resolver(ref context));
        }

        [DataTestMethod]
        [DynamicData(nameof(DependencyResolutionAttributes))]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void CircularExceptionTest(DependencyResolutionAttribute attribute)
        {
            // Arrange
            var context = new DictionaryContext() { Resolver = (t) => 
            throw new ResolutionFailedException(typeof(AttributesTests), null, null, 
                new CircularDependencyException(typeof(AttributesTests), null))};

            // Act
            _ = attribute.GetResolver<DictionaryContext>(Parameters[0])(ref context);
        }


        #region Test Data

        private const string TestValue = "d32acfb9-5836-4357-bcb7-45a4ce6db395";

        private static ParameterInfo[] Parameters = typeof(AttributesTests).GetMethod(nameof(TestMethod))
                                                                           .GetParameters();
        public string TestField = TestValue;

        public string TestProperty { get; } = TestValue;

        public void TestMethod(string param, string @default = null) { }

        public static IEnumerable<object[]> DependencyResolutionAttributes
        {
            get 
            {
                yield return new object[] { new DependencyAttribute() };
                yield return new object[] { new OptionalDependencyAttribute() };
            }
        }

        public static IEnumerable<object[]> NamedAttributes
        {
            get 
            { 
                yield return new object[] { new DependencyAttribute(TestValue) };
                yield return new object[] { new OptionalDependencyAttribute(TestValue) };
            }
        }

        #endregion
    }
}
