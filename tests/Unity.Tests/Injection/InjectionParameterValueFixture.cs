using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Injection;
using Unity.Policy;
using Unity.ResolverPolicy;
using Unity.Tests.v5.Generics;
using Unity.Tests.v5.TestSupport;

namespace Unity.Tests.v5.Injection
{
    // Tests for the DependencyValue class and its derivatives
    [TestClass]
    public class InjectionParameterValueFixture
    {

        [TestMethod]
        public void DependencyParameterCreatesExpectedResolver()
        {
            Type expectedType = typeof(ILogger);

            ResolvedParameter parameter = new ResolvedParameter<ILogger>();
            IResolverPolicy resolver = parameter.GetResolverPolicy(expectedType);

            AssertExtensions.IsInstanceOfType(resolver, typeof(NamedTypeDependencyResolverPolicy));
            Assert.AreEqual(expectedType, ((NamedTypeDependencyResolverPolicy)resolver).Type);
            Assert.IsNull(((NamedTypeDependencyResolverPolicy)resolver).Name);
        }

        [TestMethod]
        public void ResolvedParameterHandledNamedTypes()
        {
            Type expectedType = typeof(ILogger);
            string name = "special";

            ResolvedParameter parameter = new ResolvedParameter(expectedType, name);
            IResolverPolicy resolver = parameter.GetResolverPolicy(expectedType);

            AssertExtensions.IsInstanceOfType(resolver, typeof(NamedTypeDependencyResolverPolicy));
            Assert.AreEqual(expectedType, ((NamedTypeDependencyResolverPolicy)resolver).Type);
            Assert.AreEqual(name, ((NamedTypeDependencyResolverPolicy)resolver).Name);
        }

        [TestMethod]
        public void TypesImplicitlyConvertToResolvedDependencies()
        {
            List<InjectionParameterValue> values = GetParameterValues(typeof(int));

            Assert.AreEqual(1, values.Count);
            AssertExtensions.IsInstanceOfType(values[0], typeof(ResolvedParameter));
        }

        [TestMethod]
        public void TypesAndObjectsImplicitlyConvertToInjectionParameters()
        {
            List<InjectionParameterValue> values = GetParameterValues(
                15, typeof(string), 22.5);

            Assert.AreEqual(3, values.Count);
            AssertExtensions.IsInstanceOfType(values[0], typeof(InjectionParameter));
            AssertExtensions.IsInstanceOfType(values[1], typeof(ResolvedParameter));
            AssertExtensions.IsInstanceOfType(values[2], typeof(InjectionParameter));
        }

        [TestMethod]
        public void ConcreteTypesMatch()
        {
            List<InjectionParameterValue> values = GetParameterValues(typeof(int), typeof(string), typeof(User));
            Type[] expectedTypes = Sequence.Collect(typeof(int), typeof(string), typeof(User));
            for (int i = 0; i < values.Count; ++i)
            {
                Assert.IsTrue(values[i].MatchesType(expectedTypes[i]));
            }
        }

        [TestMethod]
        public void CreatingInjectionParameterWithNullValueThrows()
        {
            AssertExtensions.AssertException<ArgumentNullException>(() =>
                {
                    new InjectionParameter(null);
                });
        }

        private List<InjectionParameterValue> GetParameterValues(params object[] values)
        {
            return InjectionParameterValue.ToParameters(values).ToList();
        }
    }
}
