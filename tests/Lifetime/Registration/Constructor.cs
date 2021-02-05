using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Unity.Injection;
using Unity.Lifetime;

namespace Lifetime
{
    public partial class Managers
    {
        [TestMethod("Ctor([])"), TestProperty(INJECTING, CTOR)]
        public void Ctor_Sequence()
        {
            // Arrange
            var sequence = new InjectionMember[] { new InjectionConstructor() };

            // Act
            var manager = new TransientLifetimeManager(sequence);

            // Validate
            Assert.AreEqual(1, manager.Constructors.Count());
            Assert.IsNull(manager.Fields);
            Assert.IsNull(manager.Properties);
            Assert.IsNull(manager.Methods);
            Assert.IsNull(manager.Other);
        }

        [TestMethod("Ctor(params)"), TestProperty(INJECTING, CTOR)]
        public void Ctor_Params()
        {
            // Act
            var manager = new TransientLifetimeManager(new InjectionConstructor());

            // Validate
            Assert.AreEqual(1, manager.Constructors.Count());
            Assert.IsNull(manager.Fields);
            Assert.IsNull(manager.Properties);
            Assert.IsNull(manager.Methods);
            Assert.IsNull(manager.Other);
        }

        [TestMethod("Ctor([]{ctor,method})"), TestProperty(INJECTING, CTOR)]
        public void Ctor_Sequence_CtorMethod()
        {
            // Arrange
            var sequence = new InjectionMember[] 
            { 
                new InjectionConstructor(),
                new InjectionMethod(string.Empty) 
            };

            // Act
            var manager = new TransientLifetimeManager(sequence);

            // Validate
            Assert.AreEqual(1, manager.Constructors.Count());
            Assert.AreEqual(1, manager.Methods.Count());
            Assert.IsNull(manager.Fields);
            Assert.IsNull(manager.Properties);
            Assert.IsNull(manager.Other);
        }

        [TestMethod("Ctor(ctor,method)"), TestProperty(INJECTING, CTOR)]
        public void Ctor_Params_CtorMethod()
        {
            // Act
            var manager = new TransientLifetimeManager(
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
