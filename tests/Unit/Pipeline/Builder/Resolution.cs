using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Builder;
using Unity.Container;
using Unity.Resolution;

namespace Pipeline
{
    public partial class Builder
    {
        [Ignore]
        [TestMethod("Empty chain"), TestProperty(TEST, RESOLVE)]
        public void Resolve_FromEmpty()
        {
            // Arrange
            var builder = new PipelineBuilder<FakeContext>(Chain);

            // Act
            var visitor = builder.Build();

            // Validate
            Assert.IsNull(visitor);
        }

        [TestMethod("No overridden methods"), TestProperty(TEST, RESOLVE)]
        public void Resolve_NoStrategy()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new NoStrategy());

            // Act
            var builder = new PipelineBuilder<FakeContext>(Chain);
            var visitor = builder.Build();

            // Validate
            Assert.IsNull(visitor);
        }

        [TestMethod("Strategy with overridden PreBuildUp"), TestProperty(TEST, RESOLVE)]
        public void Resolve_PreBuildUpStrategy()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new PreBuildUpStrategy());

            // Act
            var builder = new PipelineBuilder<FakeContext>(Chain);
            var visitor = builder.Build();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
        }

        [TestMethod("Strategy with overridden PostBuildUp"), TestProperty(TEST, RESOLVE)]
        public void Resolve_PostBuildUpStrategy()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new PostBuildUpStrategy());

            // Act
            var builder = new PipelineBuilder<FakeContext>(Chain);
            var visitor = builder.Build();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
        }

        [TestMethod("Strategy with both methods overridden"), TestProperty(TEST, RESOLVE)]
        public void Resolve_BothStrategies()
        {
            // Arrange
            Chain.Add(UnityBuildStage.Creation, new BothStrategies());

            // Act
            var builder = new PipelineBuilder<FakeContext>(Chain);
            var visitor = builder.Build();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
        }

        [TestMethod("Multiple Strategies"), TestProperty(TEST, RESOLVE)]
        public void Resolve_Multiple()
        {
            // Arrange
            Chain.Add(UnityBuildStage.PreCreation, new PreBuildUpStrategy());
            Chain.Add(UnityBuildStage.Creation, new BothStrategies());
            Chain.Add(UnityBuildStage.PostCreation, new PostBuildUpStrategy());

            // Act
            var builder = new PipelineBuilder<FakeContext>(Chain);
            var visitor = builder.Build();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
        }

        [TestMethod("Strategy with fault"), TestProperty(TEST, RESOLVE)]
        public void Resolve_Faulted()
        {
            // Arrange
            Chain.Add(UnityBuildStage.PreCreation, new PreBuildUpStrategy());
            Chain.Add(UnityBuildStage.Creation, new FaultedStrategy());
            Chain.Add(UnityBuildStage.PostCreation, new PostBuildUpStrategy());

            // Act
            var builder = new PipelineBuilder<FakeContext>(Chain);
            var visitor = builder.Build();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
        }
    }
}
