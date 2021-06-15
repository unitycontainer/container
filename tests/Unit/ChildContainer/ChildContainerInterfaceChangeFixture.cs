using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity.Lifetime;

namespace Unity.Tests.v5.ChildContainer
{
    /// <summary>
    /// Summary description for TestChildContainerInterfaceChanges
    /// </summary>
    [TestClass]
    public class ChildContainerInterfaceChangeFixture
    {
        /// <summary>
        /// create parent and child container and then get the parent from child using the property parent.
        /// </summary>
        [TestMethod]
        public void CheckParentContOfChild()
        {
            IUnityContainer uc = new UnityContainer();
            IUnityContainer ucchild = uc.CreateChildContainer();
    
            object obj = ucchild.Parent;
            
            Assert.AreSame(uc, obj);
        }

        /// <summary>
        /// Check what do we get when we ask for parent's parent container
        /// </summary>
        [TestMethod]
        public void CheckParentContOfParent()
        {
            IUnityContainer uc = new UnityContainer();
            IUnityContainer ucchild = uc.CreateChildContainer();
            
            object obj = uc.Parent;
            
            Assert.IsNull(obj);
        }

        /// <summary>
        /// Check whether child inherits the configuration of the parent container or not using registertype method
        /// </summary>
        [TestMethod]
        public void ChildInheritsParentsConfiguration_RegisterTypeResolve()
        {
            IUnityContainer parent = new UnityContainer();
            parent.RegisterType<ITestContainer, TestContainer>(new ContainerControlledLifetimeManager());

            IUnityContainer child = parent.CreateChildContainer();
            ITestContainer objtest = child.Resolve<ITestContainer>();

            Assert.IsNotNull(objtest);
            Assert.IsInstanceOfType(objtest, typeof(TestContainer));
        }

        /// <summary>
        /// Check whether child inherits the configuration of the parent container or 
        /// not, using registerinstance method
        /// </summary>
        [TestMethod]
        public void ChildInheritsParentsConfiguration_RegisterInstanceResolve()
        {
            IUnityContainer parent = new UnityContainer();
            ITestContainer obj = new TestContainer();
            
            parent.RegisterInstance<ITestContainer>("InParent", obj);

            IUnityContainer child = parent.CreateChildContainer();
            ITestContainer objtest = child.Resolve<ITestContainer>("InParent");

            Assert.IsNotNull(objtest);
            Assert.AreSame(objtest, obj);
        }

        /// <summary>
        /// Check whether child inherits the configuration of the parent container or 
        /// not,using registertype method and then resolveall
        /// </summary>
        [TestMethod]
        public void ChildInheritsParentsConfiguration_RegisterTypeResolveAll()
        {
            IUnityContainer parent = new UnityContainer();
            parent.RegisterType<ITestContainer, TestContainer>()
                .RegisterType<ITestContainer, TestContainer1>("first")
                .RegisterType<ITestContainer, TestContainer2>("second");

            IUnityContainer child = parent.CreateChildContainer()
                .RegisterType<ITestContainer, TestContainer3>("third");

            List<ITestContainer> list = new List<ITestContainer>(child.ResolveAll<ITestContainer>());
            
            Assert.AreEqual(3, list.Count);
        }

        /// <summary>
        /// Check whether child inherits the configuration of the parent container or 
        /// not, Using registerinstance method and then resolveall
        /// </summary>
        [TestMethod]
        public void ChildInheritsParentsConfiguration_RegisterInstanceResolveAll()
        {
            ITestContainer objdefault = new TestContainer();
            ITestContainer objfirst = new TestContainer1();
            ITestContainer objsecond = new TestContainer2();
            ITestContainer objthird = new TestContainer3();
            IUnityContainer parent = new UnityContainer();
            
            parent.RegisterInstance<ITestContainer>(objdefault)
                .RegisterInstance<ITestContainer>("first", objfirst)
                .RegisterInstance<ITestContainer>("second", objsecond);

            IUnityContainer child = parent.CreateChildContainer()
                .RegisterInstance<ITestContainer>("third", objthird);

            List<ITestContainer> list = new List<ITestContainer>(child.ResolveAll<ITestContainer>());
            
            Assert.AreEqual(3, list.Count);
        }

        /// <summary>
        /// Register same type in parent and child and see the behavior
        /// </summary>
        [TestMethod]
        public void RegisterSameTypeInChildAndParentOverriden()
        {
            IUnityContainer parent = new UnityContainer();
            parent.RegisterType<ITestContainer, TestContainer>();
            IUnityContainer child = parent.CreateChildContainer()
                .RegisterType<ITestContainer, TestContainer1>();

            ITestContainer parentregister = parent.Resolve<ITestContainer>();
            ITestContainer childregister = child.Resolve<ITestContainer>();

            Assert.IsInstanceOfType(parentregister, typeof(TestContainer));
            Assert.IsInstanceOfType(childregister, typeof(TestContainer1));
        }

        /// <summary>
        /// Register type in parent and resolve using child.
        /// Change in parent and changes reflected in child.
        /// </summary>
        [TestMethod]
        public void ChangeInParentConfigurationIsReflectedInChild()
        {
            IUnityContainer parent = new UnityContainer();
            parent.RegisterType<ITestContainer, TestContainer>();
            IUnityContainer child = parent.CreateChildContainer();

            ITestContainer first = child.Resolve<ITestContainer>();
            parent.RegisterType<ITestContainer, TestContainer1>();
            ITestContainer second = child.Resolve<ITestContainer>();

            Assert.IsInstanceOfType(first, typeof(TestContainer));
            Assert.IsInstanceOfType(second, typeof(TestContainer1));
        }

        /// <summary>
        /// dispose parent container, child should get disposed.
        /// </summary>
        [TestMethod]
        public void WhenDisposingParentChildDisposes()
        {
            IUnityContainer parent = new UnityContainer();
            IUnityContainer child = parent.CreateChildContainer();

            TestContainer3 obj = new TestContainer3();
            child.RegisterInstance<TestContainer3>(obj);

            parent.Dispose();
            Assert.IsTrue(obj.WasDisposed);
        }

        /// <summary>
        /// dispose child, check if parent is disposed or not.
        /// </summary>
        [TestMethod]
        public void ParentNotDisposedWhenChildDisposed()
        {
            IUnityContainer parent = new UnityContainer();
            IUnityContainer child = parent.CreateChildContainer();
            TestContainer obj1 = new TestContainer();
            TestContainer3 obj3 = new TestContainer3();
            parent.RegisterInstance<TestContainer>(obj1);
            child.RegisterInstance<TestContainer3>(obj3);

            child.Dispose();
            //parent not getting disposed
            Assert.IsFalse(obj1.WasDisposed);

            //child getting disposed.
            Assert.IsTrue(obj3.WasDisposed);
        }

        [TestMethod]
        public void ChainOfContainers()
        {
            IUnityContainer parent = new UnityContainer();
            var child1 = parent.CreateChildContainer();
            var child2 = child1.CreateChildContainer();
            var child3 = child2.CreateChildContainer();

            var obj1 = new TestContainer();

            parent.RegisterInstance("InParent", obj1);
            child1.RegisterInstance("InChild1", obj1);
            child2.RegisterInstance("InChild2", obj1);
            child3.RegisterInstance("InChild3", obj1);

            object objresolve = child3.Resolve<TestContainer>("InParent");
            object objresolve1 = parent.Resolve<TestContainer>("InChild3");

            Assert.AreSame(obj1, objresolve);
            
            child1.Dispose();
            
            //parent not getting disposed
            Assert.IsTrue(obj1.WasDisposed);
        }
    }
}