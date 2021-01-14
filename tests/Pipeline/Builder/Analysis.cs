using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Container;
using Unity.Extension;

namespace Pipeline
{
    public partial class Builder
    {
        [TestMethod("Empty chain"), TestProperty(TEST, ANALYSIS)]
        public void Analysis_FromEmpty()
        {
            // Arrange
            var builder = new PipelineBuilder<FakeContext>(Chain);

            // Act
            var visitor = builder.ExpressBuildUp();

            // Validate
            Assert.IsNotNull(visitor);
            Assert.IsInstanceOfType(visitor, typeof(ResolveDelegate<FakeContext>));

            visitor(ref Context);
        }
    }
}
