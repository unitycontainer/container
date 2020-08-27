using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Unity.Policy.Tests
{
    [TestClass]
    public class PolicySetTests
    {
        [TestMethod]
        public void GetSetTest()
        {
            var set = new PolicySet();

            set.Set<PolicySetTests>(this);
            Assert.AreEqual(1, set.Count);

            Assert.AreSame(this, set.Get<PolicySetTests>());
        }
    }

    #region Test Data

    public class PolicySet : Dictionary<Type, object>, IPolicySet
    {
        public string NameField;
        public string NameProperty { get; set; }

        public PolicySet()
        {
        }

        public PolicySet(string name)
        {
            NameField = name;
            NameProperty = name;
        }

        public void TestMethod(Type @interface) => throw new NotImplementedException();

        public void Clear(Type policyInterface) => Remove(policyInterface);

        public object Get(Type policyInterface) =>
            TryGetValue(policyInterface, out object value)
                ? value : null;

        public void Set(Type policyInterface, object policy) => this[policyInterface] = policy;
    }

    #endregion
}
