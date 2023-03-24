using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity.Builder;
using Unity.Container;
using Unity.Resolution;

namespace Pipeline
{
    public partial class Behavior
    {
        [TestMethod("Empty chain"), TestProperty(TEST, BUILDUP_COMPILED)]
        public void BuildUp_Compiled_FromEmpty()
        {
            // Arrange
            // Act
            var visitor = Pipelines<FakeContext>.CompiledBuildUpPipelineFactory(Chain);

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(0, Context.Count);
        }

        [TestMethod("No overridden methods"), TestProperty(TEST, BUILDUP_COMPILED)]
        public void BuildUp_Compiled_NoStrategy()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new NoStrategy());

            // Act
            var visitor = Pipelines<FakeContext>.CompiledBuildUpPipelineFactory(Chain);

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(0, Context.Count);
        }

        [TestMethod("Strategy with overridden PreBuildUp"), TestProperty(TEST, BUILDUP_COMPILED)]
        public void BuildUp_Compiled_PreBuildUpStrategy()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new PreBuildUpStrategy());

            // Act
            var visitor = Pipelines<FakeContext>.CompiledBuildUpPipelineFactory(Chain);

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(1, Context.Count);
        }

        [TestMethod("Strategy with overridden PostBuildUp"), TestProperty(TEST, BUILDUP_COMPILED)]
        public void BuildUp_Compiled_PostBuildUpStrategy()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new PostBuildUpStrategy());

            // Act
            var visitor = Pipelines<FakeContext>.CompiledBuildUpPipelineFactory(Chain);

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(1, Context.Count);
        }

        [TestMethod("Strategy with both methods overridden"), TestProperty(TEST, BUILDUP_COMPILED)]
        public void BuildUp_Compiled_BothStrategies()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new BothStrategies());

            // Act
            var visitor = Pipelines<FakeContext>.CompiledBuildUpPipelineFactory(Chain);

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(2, Context.Count);
        }

        [TestMethod("Multiple Strategies"), TestProperty(TEST, BUILDUP_COMPILED)]
        public void BuildUp_Compiled_Multiple()
        {
            // Arrange
            Chain.Add(UnityBuildStage.PreCreation, new PreBuildUpStrategy());
            Chain.Add(UnityBuildStage.Creation, new BothStrategies());
            Chain.Add(UnityBuildStage.PostCreation, new PostBuildUpStrategy());

            // Act
            var visitor = Pipelines<FakeContext>.CompiledBuildUpPipelineFactory(Chain);

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
            Assert.AreEqual(4, Context.Count);

            var array = Context.Existing as IList<string>;
            Assert.IsNotNull(array);

            Assert.AreSame(PreBuildUpStrategy.PreName, array[0]);
            Assert.AreSame(BothStrategies.PreName, array[1]);
            Assert.AreSame(PostBuildUpStrategy.PostName, array[2]);
            Assert.AreSame(BothStrategies.PostName, array[3]);
        }

        [TestMethod("Strategy with fault"), TestProperty(TEST, BUILDUP_COMPILED)]
        public void BuildUp_Compiled_Faulted()
        {
            // Arrange
            Chain.Add(UnityBuildStage.PreCreation, new PreBuildUpStrategy());
            Chain.Add(UnityBuildStage.Creation, new FaultedStrategy());
            Chain.Add(UnityBuildStage.PostCreation, new PostBuildUpStrategy());

            // Act
            var visitor = Pipelines<FakeContext>.CompiledBuildUpPipelineFactory(Chain);

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
