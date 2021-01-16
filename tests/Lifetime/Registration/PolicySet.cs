using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Lifetime;

namespace Lifetime
{
    public partial class Managers
    {
        [TestMethod("PolicySet Baseline"), TestProperty(BASE_TYPE, POLICYSET)]
        public void PolicySet_Baseline()
        {
            // Arrange
            var manager = new TransientLifetimeManager();

            // Validate
            Assert.IsNull(manager.Constructor);
            Assert.IsNull(manager.Fields);
            Assert.IsNull(manager.Properties);
            Assert.IsNull(manager.Methods);

            Assert.IsTrue(ReferenceEquals(UnityContainer.NoValue,
                                          UnityContainer.NoValue));
        }
    }
}
