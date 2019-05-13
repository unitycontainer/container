using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

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
            var tasks = new[] 
            {
                Container.Resolve<object>(),
                Container.Resolve<object>()
            };

            // Validate
            Task.WaitAll(tasks);

            Assert.AreEqual(1, pipeline.Count);
            Assert.AreEqual(2, spy.Count);
        }

    }
}
