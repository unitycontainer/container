using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
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
        public void Factory_IsNotNull()
        {
#if UNITY_V4
            Container.RegisterType<IService>(new InjectionFactory((c, t, n) => new Service()));
#else
            Container.RegisterFactory<IService>((c, t, n) => new Service());
#endif

            Assert.IsNotNull(Container.Resolve<IService>());
        }

#if !UNITY_V4
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShortSignatureThrowsOnNull()
        {
            Func<IUnityContainer, object> factoryFunc = null;
            Container.RegisterFactory<IService>(factoryFunc);
        }
#endif

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LongSignatureThrowsOnNull()
        {
            Func<IUnityContainer, Type, string, object> factoryFunc = null;
#if UNITY_V4
            Container.RegisterType<IService>(new InjectionFactory(factoryFunc));
#else
            Container.RegisterFactory<IService>(factoryFunc);
#endif
        }
    }
}
