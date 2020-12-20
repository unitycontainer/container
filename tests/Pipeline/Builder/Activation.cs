using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Container;
using Unity.Extension;

namespace Pipeline
{
    public partial class BuilderTests
    {
        [TestMethod, TestProperty(TEST, ACTIVATE)]
        public void Activate_FromEmpty()
        {
            // Arrange
            var visitor = PipelineBuilder<FakeContext>.BuildUpPipeline(Chain);
            
            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(PipelineDelegate<FakeContext>));

            visitor(ref Context);
        }

        [TestMethod, TestProperty(TEST, ACTIVATE)]
        public void Activate_NoStrategy()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new NoStrategy());

            // Act
            var visitor = PipelineBuilder<FakeContext>.BuildUpPipeline(Chain);

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(PipelineDelegate<FakeContext>));

            visitor(ref Context);
        }

        [TestMethod, TestProperty(TEST, ACTIVATE)]
        public void Activate_PreBuildUpStrategy()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new PreBuildUpStrategy());

            // Act
            var visitor = PipelineBuilder<FakeContext>.BuildUpPipeline(Chain);

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(PipelineDelegate<FakeContext>));

            visitor(ref Context);
        }

        [TestMethod, TestProperty(TEST, ACTIVATE)]
        public void Activate_PostBuildUpStrategy()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new PostBuildUpStrategy());

            // Act
            var visitor = PipelineBuilder<FakeContext>.BuildUpPipeline(Chain);

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(PipelineDelegate<FakeContext>));

            visitor(ref Context);
        }

        [TestMethod, TestProperty(TEST, ACTIVATE)]
        public void Activate_BothStrategies()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new BothStrategies());

            // Act
            var visitor = PipelineBuilder<FakeContext>.BuildUpPipeline(Chain);

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(PipelineDelegate<FakeContext>));

            visitor(ref Context);
        }

        [TestMethod, TestProperty(TEST, ACTIVATE)]
        public void Activate_Multiple()
        {
            // Arrange
            Chain.Add(UnityBuildStage.PreCreation,  new PreBuildUpStrategy());
            Chain.Add(UnityBuildStage.Creation,     new BothStrategies());
            Chain.Add(UnityBuildStage.PostCreation, new PostBuildUpStrategy());

            // Act
            var visitor = PipelineBuilder<FakeContext>.BuildUpPipeline(Chain);

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(PipelineDelegate<FakeContext>));

            visitor(ref Context);
        }

        [TestMethod, TestProperty(TEST, ACTIVATE)]
        public void Activate_Faulted()
        {
            // Arrange
            Chain.Add(UnityBuildStage.PreCreation,  new PreBuildUpStrategy());
            Chain.Add(UnityBuildStage.Creation,     new FaultedStrategy());
            Chain.Add(UnityBuildStage.PostCreation, new PostBuildUpStrategy());

            // Act
            var visitor = PipelineBuilder<FakeContext>.BuildUpPipeline(Chain);

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(PipelineDelegate<FakeContext>));

            visitor(ref Context);
        }
    }
}
