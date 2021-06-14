using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Lifetime
{
    public partial class Managers
    {
        [TestMethod("Inject(range)"), TestProperty(BASE_TYPE, INJECTION)]
        public void Injection_InjectRangeEmpty()
        {
            // Arrange
            var sequence = new InjectionMember[]
            {
                new InjectionConstructor(),
                new InjectionMethod(string.Empty),
                new InjectionProperty(string.Empty),
                new InjectionField(string.Empty)
            };
            var manager = new TransientLifetimeManager();

            // Act
            manager.Inject(sequence);

            // Validate
            Assert.IsNotNull(manager.Constructors);
            Assert.IsNotNull(manager.Fields);
            Assert.IsNotNull(manager.Properties);
            Assert.IsNotNull(manager.Methods);
        }

        [DataTestMethod, TestProperty(BASE_TYPE, INJECTION)]
        [DynamicData(nameof(GetManagers), DynamicDataSourceType.Method)]
        public void Injection_InjectInjectionMember(RegistrationManager manager)
        {
            // Validate
            Assert.IsNull(manager.Data);
            Assert.AreEqual(RegistrationCategory.Uninitialized, manager.Category);

            // Can assign
            manager.Data = this;
            manager.Category = RegistrationCategory.Instance;
            Assert.AreSame(this, manager.Data);
            Assert.AreEqual(RegistrationCategory.Instance, manager.Category);
        }

        public static IEnumerable<object[]> GetManagers()
        {
            yield return new object[] { new ContainerControlledLifetimeManager() };
            yield return new object[] { new ContainerControlledTransientManager() };
            yield return new object[] { new ExternallyControlledLifetimeManager() };
            yield return new object[] { new HierarchicalLifetimeManager() };
            yield return new object[] { new PerResolveLifetimeManager() };
            yield return new object[] { new PerThreadLifetimeManager() };
            yield return new object[] { new TransientLifetimeManager() };
        }

    }
}
