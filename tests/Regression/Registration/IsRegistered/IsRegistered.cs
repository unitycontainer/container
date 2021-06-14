using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System.Linq;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Registration
{
    public partial class IsRegistered
    {
        [TestMethod]
        public void GenericOverload()
        {
            Assert.IsTrue(Container.IsRegistered<IUnityContainer>());
            Assert.IsFalse(Container.IsRegistered<IUnityContainer>(string.Empty));
            Assert.IsFalse(Container.IsRegistered<IUnityContainer>(Name));
        }

        [TestMethod]
        public void Type()
        {
            Assert.IsTrue(Container.IsRegistered(typeof(ILogger)));
            Assert.IsTrue(Container.IsRegistered(typeof(ILogger), Name));
            Assert.IsFalse(Container.IsRegistered(typeof(ILogger), other));
        }

        [TestMethod]
        public void Instance()
        {
            Assert.IsTrue(Container.IsRegistered(typeof(IService)));
            Assert.IsTrue(Container.IsRegistered(typeof(IService), Name));
            Assert.IsFalse(Container.IsRegistered(typeof(IService), other));
        }

        [TestMethod]
        public void Generic()
        {
            Assert.IsTrue(Container.IsRegistered(typeof(IFoo<>)));
            Assert.IsTrue(Container.IsRegistered(typeof(IFoo<>), Name));
            Assert.IsFalse(Container.IsRegistered(typeof(IFoo<>), other));
        }

        [TestMethod]
        public void FromChildContainer()
        {
            var child = Container.CreateChildContainer();

            Assert.IsTrue(child.IsRegistered(typeof(ILogger)));
            Assert.IsTrue(child.IsRegistered(typeof(ILogger), Name));
            Assert.IsFalse(child.IsRegistered(typeof(ILogger), other));
            Assert.IsTrue(child.IsRegistered(typeof(IService)));
            Assert.IsTrue(child.IsRegistered(typeof(IService), Name));
            Assert.IsFalse(child.IsRegistered(typeof(IService), other));
            Assert.IsTrue(child.IsRegistered(typeof(IFoo<>)));
            Assert.IsTrue(child.IsRegistered(typeof(IFoo<>), Name));
            Assert.IsFalse(child.IsRegistered(typeof(IFoo<>), other));
        }

        [TestMethod]
        public void FromChildChildContainer()
        {
            var child = Container.CreateChildContainer()
                                  .CreateChildContainer();

            Assert.IsTrue(child.IsRegistered(typeof(ILogger)));
            Assert.IsTrue(child.IsRegistered(typeof(ILogger), Name));
            Assert.IsFalse(child.IsRegistered(typeof(ILogger), other));
            Assert.IsTrue(child.IsRegistered(typeof(IService)));
            Assert.IsTrue(child.IsRegistered(typeof(IService), Name));
            Assert.IsFalse(child.IsRegistered(typeof(IService), other));
            Assert.IsTrue(child.IsRegistered(typeof(IFoo<>)));
            Assert.IsTrue(child.IsRegistered(typeof(IFoo<>), Name));
            Assert.IsFalse(child.IsRegistered(typeof(IFoo<>), other));
        }

        [TestMethod]
        public void WorksForRegisteredType()
        {
            Container.RegisterType<ILogger, MockLogger>();

            Assert.IsTrue(Container.IsRegistered<ILogger>());
        }
    }
}
