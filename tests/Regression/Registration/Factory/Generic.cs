using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Lifetime;
#endif

namespace Registration
{
    public partial class Factory
    {
        [TestMethod]
        public void FactoryOpenGeneric()
        {
            // Arrange
#if UNITY_V4
            Container.RegisterType(typeof(IFoo<>), new InjectionFactory((c, t, n) => new Foo<object>()));
#else
            Container.RegisterFactory(typeof(IFoo<>), (c, t, n) => new Foo<object>());
#endif
            // Act
            var result = Container.Resolve(typeof(IFoo<object>));

            // Verify
            Assert.IsNotNull(result);
        }
    }
}
