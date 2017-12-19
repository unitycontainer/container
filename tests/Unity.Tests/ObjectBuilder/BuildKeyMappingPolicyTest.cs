// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Builder;
using Unity.Policy.Mapping;

namespace Unity.Tests.ObjectBuilder
{
    [TestClass]
    public class BuildKeyMappingPolicyTest
    {
        [TestMethod]
        public void PolicyReturnsNewBuildKey()
        {
            var policy = new BuildKeyMappingPolicy(new NamedTypeBuildKey<string>());

            Assert.AreEqual(new NamedTypeBuildKey<string>(), policy.Map(new NamedTypeBuildKey<object>(), null));
        }
    }
}
