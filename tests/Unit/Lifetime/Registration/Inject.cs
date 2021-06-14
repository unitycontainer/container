using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Lifetime
{
    public partial class Managers
    {
        [TestMethod("Inject([])"), TestProperty(INJECTING, SEQUENCE)]
        public void Inject_Sequence()
        {
            // Arrange
            var sequence = new InjectionMember[] { new InjectionConstructor() };
            var manager = new TransientLifetimeManager();

            // Act
            manager.Inject(sequence);

            // Validate
            Assert.AreEqual(1, manager.Constructors.Count());
            Assert.IsNull(manager.Fields);
            Assert.IsNull(manager.Properties);
            Assert.IsNull(manager.Methods);
            Assert.IsNull(manager.Other);
        }

        [TestMethod("Inject(params)"), TestProperty(INJECTING, PARAMS)]
        public void Inject_Params()
        {
            // Arrange
            var manager = new TransientLifetimeManager();

            // Act
            manager.Inject(new InjectionConstructor());

            // Validate
            Assert.AreEqual(1, manager.Constructors.Count());
            Assert.IsNull(manager.Fields);
            Assert.IsNull(manager.Properties);
            Assert.IsNull(manager.Methods);
            Assert.IsNull(manager.Other);
        }

        [TestMethod("Inject([]{ctor,method})"), TestProperty(INJECTING, SEQUENCE)]
        public void Inject_Sequence_CtorMethod()
        {
            // Arrange
            var sequence = new InjectionMember[]
            {
                new InjectionConstructor(),
                new InjectionMethod(string.Empty)
            };
            var manager = new TransientLifetimeManager();

            // Act
            manager.Inject(sequence);

            // Validate
            Assert.AreEqual(1, manager.Constructors.Count());
            Assert.AreEqual(1, manager.Methods.Count());
            Assert.IsNull(manager.Fields);
            Assert.IsNull(manager.Properties);
            Assert.IsNull(manager.Other);
        }

        [TestMethod("Inject(ctor,method)"), TestProperty(INJECTING, PARAMS)]
        public void Inject_Params_CtorMethod()
        {
            // Arrange
            var manager = new TransientLifetimeManager();

            // Act
            manager.Inject(
                new InjectionConstructor(),
                new InjectionMethod(string.Empty));

            // Validate
            Assert.AreEqual(1, manager.Constructors.Count());
            Assert.AreEqual(1, manager.Methods.Count());
            Assert.IsNull(manager.Fields);
            Assert.IsNull(manager.Properties);
            Assert.IsNull(manager.Other);
        }
    }
}
