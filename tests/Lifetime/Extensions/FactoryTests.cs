using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Unity;
using Unity.Lifetime;

namespace Lifetime.Extensions
{
    [TestClass]
    public class RegistrationTests
    {
        [TestMethod]
        public void FactoryLifetimeTest()
        {
            var members = typeof(FactoryLifetime).GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var property in members)
            {
                var manager = property.GetValue(null);

                Assert.IsInstanceOfType(manager, typeof(IFactoryLifetimeManager));
            }
        }

        [TestMethod]
        public void InstanceLifetimeTest()
        {
            var members = typeof(InstanceLifetime).GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var property in members)
            {
                var manager = property.GetValue(null);

                Assert.IsInstanceOfType(manager, typeof(IInstanceLifetimeManager));
            }
        }


        [TestMethod]
        public void TypeLifetimeTest()
        {
            var members = typeof(TypeLifetime).GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var property in members)
            {
                var manager = property.GetValue(null);

                Assert.IsInstanceOfType(manager, typeof(ITypeLifetimeManager));
            }
        }
    }
}
