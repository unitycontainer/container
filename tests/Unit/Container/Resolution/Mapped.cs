using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Container
{
    public partial class Resolution
    {
        [TestMethod, TestProperty(RESOLVE, MAPPED)]
        public void Resolve_MappedService()
        {
            Container.RegisterType<IService, Service>();

            var instance = Container.Resolve<IService>();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service));
        }
    }
}
