// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Container;
using Unity.Strategy;
using Unity.Tests.TestSupport;

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    [TestClass]
    public class StagedStrategyChainTest
    {
        private static void AssertOrder(IStrategyChain chain,
                                params FakeStrategy[] strategies)
        {
            List<IBuilderStrategy> strategiesInChain = new List<IBuilderStrategy>(chain);
            CollectionAssertExtensions.AreEqual(strategies, strategiesInChain);
        }

        [TestMethod]
        public void InnerStrategiesComeBeforeOuterStrategiesInStrategyChain()
        {
            StagedStrategyChain<FakeStage> innerChain = new StagedStrategyChain<FakeStage>();
            StagedStrategyChain<FakeStage> outerChain = new StagedStrategyChain<FakeStage>(innerChain);
            FakeStrategy innerStrategy = new FakeStrategy();
            FakeStrategy outerStrategy = new FakeStrategy();
            innerChain.Add(innerStrategy, FakeStage.Stage1);
            outerChain.Add(outerStrategy, FakeStage.Stage1);

            IStrategyChain chain = outerChain.MakeStrategyChain();

            AssertOrder(chain, innerStrategy, outerStrategy);
        }

        [TestMethod]
        public void OrderingAcrossStagesForStrategyChain()
        {
            StagedStrategyChain<FakeStage> innerChain = new StagedStrategyChain<FakeStage>();
            StagedStrategyChain<FakeStage> outerChain = new StagedStrategyChain<FakeStage>(innerChain);
            FakeStrategy innerStage1 = new FakeStrategy { Name = "innerStage1" };
            FakeStrategy innerStage2 = new FakeStrategy { Name = "innerStage2" };
            FakeStrategy outerStage1 = new FakeStrategy { Name = "outerStage1" };
            FakeStrategy outerStage2 = new FakeStrategy { Name = "outerStage2" };
            innerChain.Add(innerStage1, FakeStage.Stage1);
            innerChain.Add(innerStage2, FakeStage.Stage2);
            outerChain.Add(outerStage1, FakeStage.Stage1);
            outerChain.Add(outerStage2, FakeStage.Stage2);

            IStrategyChain chain = outerChain.MakeStrategyChain();

            AssertOrder(chain, innerStage1, outerStage1, innerStage2, outerStage2);
        }

        [TestMethod]
        public void MultipleChildContainers()
        {
            StagedStrategyChain<FakeStage> innerChain = new StagedStrategyChain<FakeStage>();
            StagedStrategyChain<FakeStage> outerChain = new StagedStrategyChain<FakeStage>(innerChain);
            StagedStrategyChain<FakeStage> superChain = new StagedStrategyChain<FakeStage>(outerChain);

            FakeStrategy innerStrategy = new FakeStrategy { Name = "innerStrategy" };
            FakeStrategy outerStrategy = new FakeStrategy { Name = "outerStrategy" };
            FakeStrategy superStrategy = new FakeStrategy { Name = "superStrategy" };
            innerChain.Add(innerStrategy, FakeStage.Stage1);
            outerChain.Add(outerStrategy, FakeStage.Stage1);
            superChain.Add(superStrategy, FakeStage.Stage1);

            IStrategyChain chain = superChain.MakeStrategyChain();

            AssertOrder(chain, innerStrategy, outerStrategy, superStrategy);
        }

        private enum FakeStage
        {
            Stage1,
            Stage2,
        }

        private class FakeStrategy : IBuilderStrategy
        {
            public object PreBuildUp(IBuilderContext context)
            {
                throw new NotImplementedException();
            }

            public void PostBuildUp(IBuilderContext context, object pre = null)
            {
                throw new NotImplementedException();
            }

            public string Name { get; set; }
        }
    }
}
