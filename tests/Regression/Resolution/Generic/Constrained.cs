using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Resolution
{
    public partial class Generics
    {
        [TestMethod]
        public void CanResolveStructConstraintsCollections()
        {
            // Arrange
            Container.RegisterType(typeof(IGenericService<>), typeof(ServiceA<>), "A")
                     .RegisterType(typeof(IGenericService<>), typeof(ServiceB<>), "B")
                     .RegisterType(typeof(IGenericService<>), typeof(ServiceStruct<>), "Struct");

            // Act
            var result = Container.Resolve<IEnumerable<IGenericService<int>>>().ToList();
            List<IGenericService<string>> constrainedResult = Container.Resolve<IEnumerable<IGenericService<string>>>().ToList();

            // Validate
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(svc => svc is ServiceA<int>));
            Assert.IsTrue(result.Any(svc => svc is ServiceB<int>));
            Assert.IsTrue(result.Any(svc => svc is ServiceStruct<int>));

            Assert.AreEqual(2, constrainedResult.Count);
            Assert.IsTrue(constrainedResult.Any(svc => svc is ServiceA<string>));
            Assert.IsTrue(constrainedResult.Any(svc => svc is ServiceB<string>));
        }

        [TestMethod]
        public void CanResolveClassConstraintsCollections()
        {
            // Arrange
            Container.RegisterType(typeof(IGenericService<>), typeof(ServiceA<>), "A")
                     .RegisterType(typeof(IGenericService<>), typeof(ServiceB<>), "B")
                     .RegisterType(typeof(IGenericService<>), typeof(ServiceClass<>), "Class");

            // Act
            List<IGenericService<string>> result = Container.Resolve<IEnumerable<IGenericService<string>>>().ToList();
            List<IGenericService<int>> constrainedResult = Container.Resolve<IEnumerable<IGenericService<int>>>().ToList();

            // Validate
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(svc => svc is ServiceA<string>));
            Assert.IsTrue(result.Any(svc => svc is ServiceB<string>));
            Assert.IsTrue(result.Any(svc => svc is ServiceClass<string>));

            Assert.AreEqual(2, constrainedResult.Count);
            Assert.IsTrue(constrainedResult.Any(svc => svc is ServiceA<int>));
            Assert.IsTrue(constrainedResult.Any(svc => svc is ServiceB<int>));
        }

        [TestMethod]
        public void CanResolveDefaultCtorConstraintsCollections()
        {
            // Arrange
            Container.RegisterType(typeof(IGenericService<>), typeof(ServiceA<>), "A")
                     .RegisterType(typeof(IGenericService<>), typeof(ServiceB<>), "B")
                     .RegisterType(typeof(IGenericService<>), typeof(ServiceNewConstraint<>), "NewConstraint");

            // Act
            List<IGenericService<int>> result = Container.Resolve<IEnumerable<IGenericService<int>>>().ToList();
            List<IGenericService<TypeWithNoPublicNoArgCtors>> constrainedResult = Container.Resolve<IEnumerable<IGenericService<TypeWithNoPublicNoArgCtors>>>().ToList();

            // Validate
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(svc => svc is ServiceA<int>));
            Assert.IsTrue(result.Any(svc => svc is ServiceB<int>));
            Assert.IsTrue(result.Any(svc => svc is ServiceNewConstraint<int>));

            Assert.AreEqual(2, constrainedResult.Count);
            Assert.IsTrue(constrainedResult.Any(svc => svc is ServiceA<TypeWithNoPublicNoArgCtors>));
            Assert.IsTrue(constrainedResult.Any(svc => svc is ServiceB<TypeWithNoPublicNoArgCtors>));
        }

        [TestMethod]
        public void CanResolveInterfaceConstraintsCollections()
        {
            // Arrange
            Container.RegisterType(typeof(IGenericService<>), typeof(ServiceA<>), "A")
                     .RegisterType(typeof(IGenericService<>), typeof(ServiceB<>), "B")
                     .RegisterType(typeof(IGenericService<>), typeof(ServiceInterfaceConstraint<>), "InterfaceConstraint");

            // Act
            List<IGenericService<string>> result = Container.Resolve<IEnumerable<IGenericService<string>>>().ToList();
            List<IGenericService<int>> constrainedResult = Container.Resolve<IEnumerable<IGenericService<int>>>().ToList();

            // Validate
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(svc => svc is ServiceA<string>));
            Assert.IsTrue(result.Any(svc => svc is ServiceB<string>));
            Assert.IsTrue(result.Any(svc => svc is ServiceInterfaceConstraint<string>));

            Assert.AreEqual(2, constrainedResult.Count);
            Assert.IsTrue(constrainedResult.Any(svc => svc is ServiceA<int>));
            Assert.IsTrue(constrainedResult.Any(svc => svc is ServiceB<int>));
        }

    }
}
