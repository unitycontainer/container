using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests
{
    [TestClass]
    public class PipelineFixture
    {
        [TestMethod]
        public void PipelineCreatedOnlyOnce()
        {
            // Setup
            var pipeline = new SpyPipeline();
            SpyPolicy spy = new SpyPolicy();
            SpyExtension extension = new SpyExtension(pipeline, Stage.Setup, spy, typeof(SpyPolicy));

            IUnityContainerAsync Container = new UnityContainer()
                .AddExtension(extension);

            // Act
            Container.ResolveAsync<object>();
            Container.ResolveAsync<object>();

            // Validate
            Assert.AreEqual(1, pipeline.Count);
            Assert.AreEqual(2, spy.Count);
        }

    }
}
