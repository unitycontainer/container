// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Builder;
using Unity.ObjectBuilder.Strategies;
using Unity.Policy;

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    [TestClass]
    public class BuildPlanStrategyFixture
    {
        [TestMethod]
        public void StrategyGetsBuildPlanFromPolicySet()
        {
            MockBuilderContext context = new MockBuilderContext();
            context.Strategies.Add(new BuildPlanStrategy());
            object instance = new object();
            ReturnInstanceBuildPlan plan = new ReturnInstanceBuildPlan(instance);

            context.Policies.Set(typeof(object), null, typeof(IBuildPlanPolicy), plan);

            object result = context.ExecuteBuildUp(new NamedTypeBuildKey<object>(), null);

            Assert.IsTrue(plan.BuildUpCalled);
            Assert.AreSame(instance, result);
        }

        [TestMethod]
        public void StrategyCreatesBuildPlanWhenItDoesntExist()
        {
            MockBuilderContext context = new MockBuilderContext();
            context.Strategies.Add(new BuildPlanStrategy());
            MockBuildPlanCreatorPolicy policy = new MockBuildPlanCreatorPolicy();
            context.Policies.Set(null, null, typeof(IBuildPlanCreatorPolicy),policy);

            object result = context.ExecuteBuildUp(new NamedTypeBuildKey<object>(), null);

            Assert.IsNotNull(result);
            Assert.IsTrue(policy.PolicyWasCreated);

            var plan = context.Policies.GetOrDefault(typeof(IBuildPlanPolicy), new NamedTypeBuildKey(typeof(object)), out _);
            Assert.IsNotNull(plan);
        }
    }

    internal class MockBuildPlanCreatorPolicy : IBuildPlanCreatorPolicy
    {
        private bool policyWasCreated = false;

        public IBuildPlanPolicy CreatePlan(IBuilderContext context, INamedType buildKey)
        {
            policyWasCreated = true;
            return new ReturnInstanceBuildPlan(new object());
        }

        public bool PolicyWasCreated
        {
            get { return policyWasCreated; }
            set { policyWasCreated = value; }
        }
    }

    internal class ReturnInstanceBuildPlan : IBuildPlanPolicy
    {
        private object instance;
        private bool buildUpCalled;

        public ReturnInstanceBuildPlan(object instance)
        {
            this.instance = instance;
            this.buildUpCalled = false;
        }

        public void BuildUp(IBuilderContext context)
        {
            buildUpCalled = true;
            context.Existing = instance;
        }

        public bool BuildUpCalled
        {
            get { return buildUpCalled; }
        }

        public object Instance
        {
            get { return instance; }
        }
    }
}
