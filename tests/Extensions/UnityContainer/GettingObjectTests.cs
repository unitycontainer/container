using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;

namespace Extensions.Tests
{
    public partial class UnityContainerTests
    {
        #region Resolve

        [TestMethod]
        public void ResolveGeneric()
        {
            // Arrange
            container.Data = container;

            // Act
            container.Resolve<IUnityContainer>();

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNotNull(container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.Resolve<IUnityContainer>());
        }

        [TestMethod]
        public void ResolveGenericWithName()
        {
            // Arrange
            container.Data = container;

            // Act
            container.Resolve<IUnityContainer>(name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.AreEqual(name, container.Name);
            Assert.IsNotNull(container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.Resolve<IUnityContainer>(name));
        }

        [TestMethod]
        public void Resolve()
        {
            // Arrange
            container.Data = container;

            // Act
            container.Resolve(typeof(IUnityContainer));

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNotNull(container.Data);   // TODO: should be null
            Assert.ThrowsException<ArgumentNullException>(() => unity.Resolve(typeof(IUnityContainer)));
        }

        #endregion


        #region Resolve All

        [TestMethod]
        public void ResolveAllGeneric()
        {
            // Arrange
            container.Data = new object[0];

            // Act
            container.ResolveAll<IUnityContainer>();

            // Validate
            Assert.AreEqual(typeof(IUnityContainer[]), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNotNull(container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.ResolveAll<IUnityContainer>());
        }

        [TestMethod]
        public void ResolveAll()
        {
            // Arrange
            container.Data = new bool[] { true, true, false } as object;

            // Act
            container.ResolveAll(typeof(bool));

            // Validate
            Assert.AreEqual(typeof(bool[]), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNotNull(container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.ResolveAll(typeof(IUnityContainer)));
            Assert.ThrowsException<ArgumentNullException>(() => container.ResolveAll(null));
        }

        [TestMethod]
        public void ResolveAllEnumerable()
        {
            // Arrange
            container.Data = new int[] { 0, 1 };

            // Act
            container.ResolveAll(typeof(int));

            // Validate
            Assert.AreEqual(typeof(int[]), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNotNull(container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.ResolveAll(typeof(int)));
            Assert.ThrowsException<ArgumentNullException>(() => container.ResolveAll(null));
        }

        #endregion


        #region BuildUp

        [TestMethod]
        public void BuildUpGeneric()
        {
            // Act
            container.BuildUp<IUnityContainer>(container);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNotNull(container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.BuildUp<IUnityContainer>(container));
            Assert.ThrowsException<ArgumentNullException>(() => container.BuildUp<IUnityContainer>(null));
        }

        [TestMethod]
        public void BuildUpWithNameGeneric()
        {
            // Act
            container.BuildUp<IUnityContainer>(container, name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.AreEqual(name, container.Name);
            Assert.IsNotNull(container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.BuildUp<IUnityContainer>(container, name));
            Assert.ThrowsException<ArgumentNullException>(() => container.BuildUp<IUnityContainer>(null, name));
        }

        [TestMethod]
        public void BuildUp()
        {
            // Act
            container.BuildUp(typeof(IUnityContainer), container);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNotNull(container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.BuildUp(typeof(IUnityContainer), container));
            // TODO:  Assert.ThrowsException<ArgumentNullException>(() => container.BuildUp(typeof(IUnityContainer), (object)null));
        }

        #endregion
    }
}
