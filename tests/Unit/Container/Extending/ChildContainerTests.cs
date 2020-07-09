using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Container.Extending
{
    [TestClass]
    public class ChildContainerTests
    {
        IUnityContainer Container;


        [TestInitialize]
        public void TestInitialize()
        {
            Container  = new UnityContainer();
        }


        [TestMethod]
        public void CreateEventInChild()
        {
            // Arrange
            var extension = new SubscriberExtension();

            var level_two = Container.CreateChildContainer()
                                     .AddExtension(extension);
            // Validate
            Assert.IsNull(extension.ChildContext);

            // Act
            _ = Container.CreateChildContainer();
            Assert.IsNull(extension.ChildContext);

            // Validate
            var child = level_two.CreateChildContainer();
            Assert.IsNotNull(extension.ChildContext);

            Assert.AreSame(child, extension.ChildContext.Container);
            Assert.AreNotSame(level_two, extension.ChildContext.Container);
        }
    }
}
