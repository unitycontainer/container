// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Builder;
using Unity.Container;
using Unity.Policy;
using Unity.Storage;

namespace Unity.Tests.ObjectBuilder
{
    [TestClass]
    public class PolicyListTest
    {

        [TestMethod]
        public void CanAddPolicyToBagAndRetrieveIt()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy = new FakePolicy();
            list.Set(typeof(object), string.Empty, typeof(IBuilderPolicy), policy);

            IBuilderPolicy result = list.GetOrDefault(typeof(IBuilderPolicy), typeof(object), out _);

            Assert.AreSame(policy, result);
        }

        [TestMethod]
        public void CanClearDefaultPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy defaultPolicy = new FakePolicy();
            list.Set(null, null, typeof(IBuilderPolicy), defaultPolicy);

            list.Clear(null, null, typeof(IBuilderPolicy));

            IBuilderPolicy result = list.GetOrDefault(typeof(IBuilderPolicy), typeof(object), out _);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void CanClearPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy = new FakePolicy();

            list.Set(typeof(string), string.Empty, typeof(IBuilderPolicy),  policy);
            list.Clear(typeof(string) , string.Empty, typeof(IBuilderPolicy));

            Assert.IsNull(list.GetOrDefault(typeof(IBuilderPolicy), typeof(string), out _));
        }

        [TestMethod]
        public void CanRegisterGenericPolicyAndRetrieveWithSpecificGenericInstance()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy = new FakePolicy();
            list.Set(typeof(IDummy<>), string.Empty, typeof(FakePolicy), policy);

            var result = list.GetOrDefault(typeof(FakePolicy), typeof(IDummy<int>), out _);

            Assert.AreSame(policy, result);
        }

        [TestMethod]
        public void DefaultPolicyUsedWhenSpecificPolicyIsntAvailable()
        {
            PolicyList list = new PolicyList();
            FakePolicy defaultPolicy = new FakePolicy();
            list.Set(null, null, typeof(IBuilderPolicy), defaultPolicy);

            IBuilderPolicy result = list.GetOrDefault(typeof(IBuilderPolicy), typeof(object), out _);

            Assert.AreSame(defaultPolicy, result);
        }

        [TestMethod]
        public void PolicyRegisteredForTypeIsUsedIfKeyIsNotFound()
        {
            PolicyList list = new PolicyList();
            FakePolicy policyForType = new FakePolicy();
            list.Set(typeof(object), string.Empty, typeof(IBuilderPolicy), policyForType);

            IBuilderPolicy result = list.GetOrDefault(typeof(IBuilderPolicy), new NamedTypeBuildKey<object>("name"), out _);

            Assert.AreSame(policyForType, result);
        }

        [TestMethod]
        public void PolicyForClosedGenericTypeOverridesPolicyForOpenType()
        {
            PolicyList list = new PolicyList();
            FakePolicy openTypePolicy = new FakePolicy();
            FakePolicy closedTypePolicy = new FakePolicy();
            list.Set(typeof(IDummy<>), string.Empty, typeof(IBuilderPolicy), openTypePolicy);
            list.Set(typeof(IDummy<object>), string.Empty, typeof(IBuilderPolicy), closedTypePolicy);

            IBuilderPolicy result = list.GetOrDefault(typeof(IBuilderPolicy), new NamedTypeBuildKey<IDummy<object>>("name"), out _);
            Assert.AreSame(closedTypePolicy, result);
        }

        [TestMethod]
        public void PolicyRegisteredForOpenGenericTypeUsedIfKeyIsNotFound()
        {
            PolicyList list = new PolicyList();
            FakePolicy policyForType = new FakePolicy();
            list.Set(typeof(IDummy<>), string.Empty, typeof(IBuilderPolicy), policyForType);

            IBuilderPolicy result = list.GetOrDefault(typeof(IBuilderPolicy), new NamedTypeBuildKey<IDummy<object>>("name"), out _);
            Assert.AreSame(policyForType, result);
        }

        [TestMethod]
        public void OuterPolicyDefaultOverridesInnerPolicyDefault()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy innerPolicy = new FakePolicy();
            FakePolicy outerPolicy = new FakePolicy();
            innerList.Set(null, null, typeof(FakePolicy), innerPolicy);
            outerList.Set(null, null, typeof(FakePolicy), outerPolicy);

            IPolicyList containingPolicyList;
            var result = outerList.GetOrDefault(typeof(FakePolicy), typeof(object), out containingPolicyList);

            Assert.AreSame(outerPolicy, result);
            Assert.AreSame(outerList, containingPolicyList);
        }

        [TestMethod]
        public void OuterPolicyOverridesInnerPolicy()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy innerPolicy = new FakePolicy();
            FakePolicy outerPolicy = new FakePolicy();
            innerList.Set(typeof(object), string.Empty, typeof(FakePolicy), innerPolicy);
            outerList.Set(typeof(object), string.Empty, typeof(FakePolicy), outerPolicy);

            IPolicyList containingPolicyList;
            var result = outerList.GetOrDefault(typeof(FakePolicy), typeof(object), out containingPolicyList);

            Assert.AreSame(outerPolicy, result);
            Assert.AreSame(outerList, containingPolicyList);
        }

        [TestMethod]
        public void SetOverwritesExistingPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy1 = new FakePolicy();
            FakePolicy policy2 = new FakePolicy();
            list.Set(typeof(string), string.Empty, typeof(IBuilderPolicy), policy1);
            list.Set(typeof(string), string.Empty, typeof(IBuilderPolicy), policy2);

            IBuilderPolicy result = list.GetOrDefault(typeof(IBuilderPolicy), typeof(string), out _);

            Assert.AreSame(policy2, result);
        }

        [TestMethod]
        public void SpecificGenericPolicyComesBeforeGenericPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy genericPolicy = new FakePolicy();
            FakePolicy specificPolicy = new FakePolicy();
            list.Set(typeof(IDummy<>), string.Empty, typeof(FakePolicy), genericPolicy);
            list.Set(typeof(IDummy<int>), string.Empty, typeof(FakePolicy), specificPolicy);

            var result = list.GetOrDefault(typeof(FakePolicy), typeof(IDummy<int>), out _);

            Assert.AreSame(specificPolicy, result);
        }

        [TestMethod]
        public void SpecificInnerPolicyOverridesDefaultOuterPolicy()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy innerPolicy = new FakePolicy();
            FakePolicy outerPolicy = new FakePolicy();
            innerList.Set(typeof(object), string.Empty, typeof(FakePolicy), innerPolicy);
            outerList.Set(null, null, typeof(FakePolicy), outerPolicy);

            IPolicyList containingPolicyList;
            var result = outerList.GetOrDefault(typeof(FakePolicy), typeof(object), out containingPolicyList);

            Assert.AreSame(innerPolicy, result);
            Assert.AreSame(innerList, containingPolicyList);
        }

        [TestMethod]
        public void SpecificPolicyOverridesDefaultPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy defaultPolicy = new FakePolicy();
            FakePolicy specificPolicy = new FakePolicy();
            list.Set(typeof(object), string.Empty, typeof(IBuilderPolicy), specificPolicy);
            list.Set(null, null, typeof(IBuilderPolicy), defaultPolicy);

            IBuilderPolicy result = list.GetOrDefault(typeof(IBuilderPolicy), typeof(object), out _);

            Assert.AreSame(specificPolicy, result);
        }

        [TestMethod]
        public void WillAskInnerPolicyListWhenOuterHasNoAnswer()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy policy = new FakePolicy();
            innerList.Set(typeof(object), string.Empty, typeof(FakePolicy), policy);

            IPolicyList containingPolicies;
            var result = outerList.GetOrDefault(typeof(FakePolicy), typeof(object), out containingPolicies);

            Assert.AreSame(policy, result);
            Assert.AreSame(innerList, containingPolicies);
        }

        [TestMethod]
        public void WillUseInnerDefaultPolicyWhenOuterHasNoAnswer()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy policy = new FakePolicy();
            innerList.Set(null, null, typeof(FakePolicy), policy);

            IPolicyList containingPolicyList;
            var result = outerList.GetOrDefault(typeof(FakePolicy), typeof(object), out containingPolicyList);

            Assert.AreSame(policy, result);
            Assert.AreSame(innerList, containingPolicyList);
        }

        private class FakePolicy : IBuilderPolicy { }

        private interface IDummy<T> { }
    }
}
