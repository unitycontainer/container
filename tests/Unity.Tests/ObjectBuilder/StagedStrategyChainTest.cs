using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity.Storage;
using Unity.Strategies;
using Unity.Tests.v5.TestSupport;

namespace Unity.Tests.v5.ObjectBuilder
{
    [TestClass]
    public class StagedStrategyChainTest
    {
        private static void AssertOrder(IEnumerable<BuilderStrategy> chain,
                                params FakeStrategy[] strategies)
        {
            List<BuilderStrategy> strategiesInChain = new List<BuilderStrategy>(chain);
            CollectionAssertExtensions.AreEqual(strategies, strategiesInChain);
        }

        [TestMethod]
        public void InnerStrategiesComeBeforeOuterStrategiesInStrategyChain()
        {
            StagedStrategyChain<BuilderStrategy, FakeStage> innerChain = new StagedStrategyChain<BuilderStrategy, FakeStage>();
            StagedStrategyChain<BuilderStrategy, FakeStage> outerChain = new StagedStrategyChain<BuilderStrategy, FakeStage>(innerChain);
            FakeStrategy innerStrategy = new FakeStrategy();
            FakeStrategy outerStrategy = new FakeStrategy();
            innerChain.Add(innerStrategy, FakeStage.Stage1);
            outerChain.Add(outerStrategy, FakeStage.Stage1);

            var chain = outerChain.ToArray();

            AssertOrder(chain, innerStrategy, outerStrategy);
        }

        [TestMethod]
        public void OrderingAcrossStagesForStrategyChain()
        {
            StagedStrategyChain<BuilderStrategy, FakeStage> innerChain = new StagedStrategyChain<BuilderStrategy, FakeStage>();
            StagedStrategyChain<BuilderStrategy, FakeStage> outerChain = new StagedStrategyChain<BuilderStrategy, FakeStage>(innerChain);
            FakeStrategy innerStage1 = new FakeStrategy { Name = "innerStage1" };
            FakeStrategy innerStage2 = new FakeStrategy { Name = "innerStage2" };
            FakeStrategy outerStage1 = new FakeStrategy { Name = "outerStage1" };
            FakeStrategy outerStage2 = new FakeStrategy { Name = "outerStage2" };
            innerChain.Add(innerStage1, FakeStage.Stage1);
            innerChain.Add(innerStage2, FakeStage.Stage2);
            outerChain.Add(outerStage1, FakeStage.Stage1);
            outerChain.Add(outerStage2, FakeStage.Stage2);

            var chain = outerChain.ToArray();

            AssertOrder(chain, innerStage1, outerStage1, innerStage2, outerStage2);
        }

        [TestMethod]
        public void MultipleChildContainers()
        {
            StagedStrategyChain<BuilderStrategy, FakeStage> innerChain = new StagedStrategyChain<BuilderStrategy, FakeStage>();
            StagedStrategyChain<BuilderStrategy, FakeStage> outerChain = new StagedStrategyChain<BuilderStrategy, FakeStage>(innerChain);
            StagedStrategyChain<BuilderStrategy, FakeStage> superChain = new StagedStrategyChain<BuilderStrategy, FakeStage>(outerChain);

            FakeStrategy innerStrategy = new FakeStrategy { Name = "innerStrategy" };
            FakeStrategy outerStrategy = new FakeStrategy { Name = "outerStrategy" };
            FakeStrategy superStrategy = new FakeStrategy { Name = "superStrategy" };
            innerChain.Add(innerStrategy, FakeStage.Stage1);
            outerChain.Add(outerStrategy, FakeStage.Stage1);
            superChain.Add(superStrategy, FakeStage.Stage1);

            var chain = superChain.ToArray();

            AssertOrder(chain, innerStrategy, outerStrategy, superStrategy);
        }

        private enum FakeStage
        {
            Stage1,
            Stage2,
        }

        private class FakeStrategy : BuilderStrategy
        {
            public string Name { get; set; }
        }
    }
}
