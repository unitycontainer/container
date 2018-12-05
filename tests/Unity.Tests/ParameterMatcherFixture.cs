using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity.Tests.v5.Generics;
using Unity.Utility;

namespace Unity.Tests.v5
{
    /// <summary>
    /// Tests around the 
    /// </summary>
    [TestClass]
    public class ParameterMatcherFixture
    {
        [TestMethod]
        public void EmptyParameterListMatches()
        {
            Assert.IsTrue(Parameters().Matches(Types()));
        }

        [TestMethod]
        public void MismatchedParameterListsDontMatch()
        {
            Assert.IsFalse(Parameters().Matches(Types(typeof(int))));
        }

        [TestMethod]
        public void SameLengthDifferentTypesDontMatch()
        {
            Assert.IsFalse(Parameters(typeof(int)).Matches(Types(typeof(string))));
        }

        [TestMethod]
        public void SameLengthSameTypesMatch()
        {
            Assert.IsTrue(Parameters(typeof(int), typeof(string)).Matches(Types(typeof(int), typeof(string))));
        }

        [TestMethod]
        public void OpenGenericTypesMatch()
        {
            Assert.IsTrue(Parameters(typeof(ICommand<>), typeof(ICommand<>)).Matches(Types(typeof(ICommand<>), typeof(ICommand<>))));
        }

        private static InjectionParameterValue[] Parameters(params Type[] types)
        {
            List<InjectionParameterValue> values = new List<InjectionParameterValue>();
            foreach (Type t in types)
            {
                values.Add(InjectionParameterValue.ToParameter(t));
            }
            return values.ToArray();
        }

        private static Type[] Types(params Type[] types)
        {
            return types;
        }
    }
}
