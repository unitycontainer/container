using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Container;
using Unity.Extension;

namespace Pipeline
{
    public partial class Builder
    {
        [TestMethod("Empty chain"), TestProperty(TEST, EXPRESS)]
        public void Express_FromEmpty()
        {
            // Arrange
            var builder = new PipelineBuilder<FakeContext>(Chain);

            // Act
            var visitor = builder.Compile();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
        }

        [TestMethod("No overridden methods"), TestProperty(TEST, EXPRESS)]
        public void Express_NoStrategy()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new NoStrategy());

            // Act
            var builder = new PipelineBuilder<FakeContext>(Chain);
            var visitor = builder.Compile();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
        }

        [TestMethod("Strategy with overridden PreBuildUp"), TestProperty(TEST, EXPRESS)]
        public void Express_PreBuildUpStrategy()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new PreBuildUpStrategy());

            // Act
            var builder = new PipelineBuilder<FakeContext>(Chain);
            var visitor = builder.Compile();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
        }

        [TestMethod("Strategy with overridden PostBuildUp"), TestProperty(TEST, EXPRESS)]
        public void Express_PostBuildUpStrategy()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new PostBuildUpStrategy());

            // Act
            var builder = new PipelineBuilder<FakeContext>(Chain);
            var visitor = builder.Compile();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
        }

        [TestMethod("Strategy with both methods overridden"), TestProperty(TEST, EXPRESS)]
        public void Express_BothStrategies()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new BothStrategies());

            // Act
            var builder = new PipelineBuilder<FakeContext>(Chain);
            var visitor = builder.Compile();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
        }

        [TestMethod("Multiple Strategies"), TestProperty(TEST, EXPRESS)]
        public void Express_Multiple()
        {
            // Arrange
            Chain.Add(UnityBuildStage.PreCreation, new PreBuildUpStrategy());
            Chain.Add(UnityBuildStage.Creation, new BothStrategies());
            Chain.Add(UnityBuildStage.PostCreation, new PostBuildUpStrategy());

            // Act
            var builder = new PipelineBuilder<FakeContext>(Chain);
            var visitor = builder.Compile();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
        }

        [TestMethod("Strategy with fault"), TestProperty(TEST, EXPRESS)]
        public void Express_Faulted()
        {
            // Arrange
            Chain.Add(UnityBuildStage.PreCreation, new PreBuildUpStrategy());
            Chain.Add(UnityBuildStage.Creation, new FaultedStrategy());
            Chain.Add(UnityBuildStage.PostCreation, new PostBuildUpStrategy());

            // Act
            var builder = new PipelineBuilder<FakeContext>(Chain);
            var visitor = builder.Compile();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
        }
    }
}
