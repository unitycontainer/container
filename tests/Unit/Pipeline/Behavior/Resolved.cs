using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity.Builder;
using Unity.Container;
using Unity.Resolution;

namespace Pipeline
{
    public partial class Behavior
    {
        [TestMethod("Empty chain"), TestProperty(TEST, BUILDUP_RESOLVED)]
        public void Buildup_Resolved_FromEmpty()
        {
            // Arrange
            var visitor = Pipelines<FakeContext>.ResolvedChainToPipelineFactory(Chain);

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(0, Context.Count);
        }

        [TestMethod("Strategy with overridden PreBuildUp"), TestProperty(TEST, BUILDUP_RESOLVED)]
        public void Buildup_Resolved_PreBuildUpStrategy()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new PreBuildUpStrategy().PreBuildUp);

            // Act
            var visitor = Pipelines<FakeContext>.ResolvedChainToPipelineFactory(Chain);

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(1, Context.Count);
        }

        [TestMethod("Strategy with overridden PostBuildUp"), TestProperty(TEST, BUILDUP_RESOLVED)]
        public void Buildup_Resolved_PostBuildUpStrategy()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new PostBuildUpStrategy().PostBuildUp);

            // Act
            var visitor = Pipelines<FakeContext>.ResolvedChainToPipelineFactory(Chain);

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(1, Context.Count);
        }

        [TestMethod("Strategy with both methods overridden"), TestProperty(TEST, BUILDUP_RESOLVED)]
        public void Buildup_Resolved_BothStrategies()
        {
            // Arrange
            Chain.Add(UnityBuildStage.PreCreation, new BothStrategies().PreBuildUp);
            Chain.Add(UnityBuildStage.PostCreation, new BothStrategies().PostBuildUp);

            // Act
            var visitor = Pipelines<FakeContext>.ResolvedChainToPipelineFactory(Chain);

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(2, Context.Count);
        }

        [TestMethod("Multiple Strategies"), TestProperty(TEST, BUILDUP_RESOLVED)]
        public void Buildup_Resolved_Multiple()
        {
            var both = new BothStrategies();

            // Arrange
            Chain.Add(UnityBuildStage.Setup, new PreBuildUpStrategy().PreBuildUp);
            Chain.Add(UnityBuildStage.PreCreation, both.PreBuildUp);
            Chain.Add(UnityBuildStage.PostCreation, both.PostBuildUp);
            Chain.Add(UnityBuildStage.PostInitialization, new PostBuildUpStrategy().PostBuildUp);

            // Act
            var visitor = Pipelines<FakeContext>.ResolvedChainToPipelineFactory(Chain);

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(4, Context.Count);

            var array = Context.Existing as IList<string>;
            Assert.IsNotNull(array);

            Assert.AreSame(PreBuildUpStrategy.PreName, array[0]);
            Assert.AreSame(BothStrategies.PreName, array[1]);
            Assert.AreSame(BothStrategies.PostName, array[2]);
            Assert.AreSame(PostBuildUpStrategy.PostName, array[3]);
        }

        [TestMethod("Strategy with fault"), TestProperty(TEST, BUILDUP_RESOLVED)]
        public void Buildup_Resolved_Faulted()
        {
            // Arrange
            Chain.Add(UnityBuildStage.PreCreation, new PreBuildUpStrategy().PreBuildUp);
            Chain.Add(UnityBuildStage.Creation, new FaultedStrategy().PreBuildUp);
            Chain.Add(UnityBuildStage.PostCreation, new PostBuildUpStrategy().PostBuildUp);

            // Act
            var visitor = Pipelines<FakeContext>.ResolvedChainToPipelineFactory(Chain);

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(2, Context.Count);

            var array = Context.Existing as IList<string>;
            Assert.IsNotNull(array);

            Assert.AreSame(PreBuildUpStrategy.PreName, array[0]);
            Assert.AreSame(FaultedStrategy.PreName, array[1]);
        }
    }
}
