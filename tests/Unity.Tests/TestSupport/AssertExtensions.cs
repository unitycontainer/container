// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.TestSupport
{
    public static class AssertExtensions
    {
        public static void AssertException<TException>(Action action)
            where TException : Exception
        {
            AssertException<TException>(action, (e) => { });
        }

        public static void AssertException<TException>(Action action, Action<TException> callback)
           where TException : Exception
        {
            try
            {
                action();
                Assert.Fail("Expected exception of type {0}", typeof(TException).GetTypeInfo().Name);
            }
            catch (TException e)
            {
                callback(e);
            }
        }

        public static void IsInstanceOfType(object value, Type expectedType)
        {
            Assert.IsNotNull(value, "value should not be null");
            Assert.IsNotNull(value, "expectedType should not be null");
            Assert.IsTrue(expectedType.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()));
        }
    }
}
