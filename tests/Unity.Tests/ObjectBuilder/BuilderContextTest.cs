// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.ObjectBuilder;
using Unity.Strategy;

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    [TestClass]
    public class BuilderContextTest : IBuilderStrategy
    {
        private IBuilderContext parentContext, childContext, receivedContext;
        private bool throwOnBuildUp;
        private IUnityContainer container = new UnityContainer();

        [TestInitialize]
        public void SetUp()
        {
            this.throwOnBuildUp = false;
        }

        [TestMethod]
        public void NewBuildSetsChildContextWhileBuilding()
        {
            this.parentContext = new BuilderContext(container, GetNonThrowingStrategyChain(), null, null, null, null, null);

            this.parentContext.NewBuildUp(null);

            Assert.AreSame(this.childContext, this.receivedContext);
        }

        [TestMethod]
        public void NewBuildClearsTheChildContextOnSuccess()
        {
            this.parentContext = new BuilderContext(container, GetNonThrowingStrategyChain(), null, null, null, null, null);

            this.parentContext.NewBuildUp(null);

            Assert.IsNull(this.parentContext.ChildContext);
        }

        [TestMethod]
        public void NewBuildDoesNotClearTheChildContextOnFailure()
        {
            this.parentContext = new BuilderContext(container, GetThrowingStrategyChain(), null, null, null, null, null);

            try
            {
                this.parentContext.NewBuildUp(null);
                Assert.Fail("an exception should have been thrown here");
            }
            catch (Exception)
            {
                Assert.IsNotNull(this.parentContext.ChildContext);
                Assert.AreSame(this.parentContext.ChildContext, this.receivedContext);
            }
        }

        private MockStrategyChain GetNonThrowingStrategyChain()
        {
            this.throwOnBuildUp = false;
            return new MockStrategyChain(new[] { this });
        }

        private MockStrategyChain GetThrowingStrategyChain()
        {
            this.throwOnBuildUp = true;
            return new MockStrategyChain(new[] { this });
        }

        public object PreBuildUp(IBuilderContext context)
        {
            this.childContext = this.parentContext.ChildContext;
            this.receivedContext = context;

            if (this.throwOnBuildUp)
            {
                throw new Exception();
            }

            return null;
        }

        public void PostBuildUp(IBuilderContext context, object pre = null)
        {
        }
    }
}
