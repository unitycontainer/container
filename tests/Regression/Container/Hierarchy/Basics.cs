using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Collections.Generic;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Lifetime;
#endif

namespace Container
{
    public partial class Basics
    {
        [PatternTestMethod("IUnityContainer Resolves Itself"), TestCategory(nameof(IUnityContainer))]
        public void IUnityContainer_Itself()
        {
            IUnityContainer resolvedContainer = Container.Resolve<IUnityContainer>();

            Assert.AreSame(Container, resolvedContainer);
        }

        [PatternTestMethod("Child container resolves itself"), TestCategory(nameof(IUnityContainer))]
        public void IUnityContainer_ChildContainer()
        {
            IUnityContainer childContainer1 = Container.CreateChildContainer();
            IUnityContainer childContainer2 = childContainer1.CreateChildContainer();
            IUnityContainer childContainer3 = childContainer2.CreateChildContainer();

            IUnityContainer resolvedContainer1 = childContainer1.Resolve<IUnityContainer>();
            IUnityContainer resolvedContainer2 = childContainer2.Resolve<IUnityContainer>();
            IUnityContainer resolvedContainer3 = childContainer3.Resolve<IUnityContainer>();

            Assert.AreSame(childContainer1, resolvedContainer1);
            Assert.AreSame(childContainer2, resolvedContainer2);
            Assert.AreSame(childContainer3, resolvedContainer3);
        }



        [TestMethod]
        public void ChildContainersAreAllowedToBeCollectedWhenDisposed()
        {
            var wr = GetWeakReferenceToChildContainer();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(wr.IsAlive);

            WeakReference GetWeakReferenceToChildContainer()
            {
                var child = Container.CreateChildContainer();
                var weak = new WeakReference(child);
                child.Dispose();

                return weak;
            }
        }


        [TestMethod]
        public void DisposingParentDisposesChild()
        {
            var parent = Container;
            var child = parent.CreateChildContainer();

            MyDisposableObject spy = new MyDisposableObject();
            child.RegisterInstance(spy);
            parent.Dispose();

            Assert.IsTrue(spy.WasDisposed);
        }

        [TestMethod]
        public void CanDisposeChildWithoutDisposingParent()
        {
            MyDisposableObject parentSpy = new MyDisposableObject();
            MyDisposableObject childSpy = new MyDisposableObject();
            var parent = Container;

            parent.RegisterInstance(parentSpy);
            var child = parent.CreateChildContainer()
                              .RegisterInstance(childSpy);
            child.Dispose();

            Assert.IsFalse(parentSpy.WasDisposed);
            Assert.IsTrue(childSpy.WasDisposed);

            childSpy.WasDisposed = false;
            parent.Dispose();

            Assert.IsTrue(parentSpy.WasDisposed);
            Assert.IsFalse(childSpy.WasDisposed);
        }

        [TestMethod]
        public void VerifyToList()
        {
            string[] numbers = { "first", "second", "third" };
            var parent = Container;

            parent.RegisterInstance(numbers[0], "first")
                .RegisterInstance(numbers[1], "second");
            var child = parent.CreateChildContainer()
                .RegisterInstance(numbers[2], "third");

            List<string> nums = new List<string>(child.ResolveAll<string>());
            CollectionAssert.AreEquivalent(numbers, nums);
        }

        [TestMethod]
        public void DuplicateRegInParentAndChild()
        {
            string[] numbers = { "first", "second", "third", "fourth" };

            var parent = Container;
            parent.RegisterInstance(numbers[0], "1")
                  .RegisterInstance(numbers[1], "2");

            var child = parent.CreateChildContainer();

            List<string> childnums = new List<string>(child.ResolveAll<string>());
            List<string> parentnums = new List<string>(parent.ResolveAll<string>());

            CollectionAssert.AreEquivalent(childnums, parentnums);

            parent.RegisterInstance(numbers[3], "4");
            child.RegisterInstance(numbers[3], "4");

            List<string> childnums2 = new List<string>(child.ResolveAll<string>());
            List<string> parentnums2 = new List<string>(parent.ResolveAll<string>());

            CollectionAssert.AreEquivalent(childnums2, parentnums2); //Both parent child should have same instances
        }

        [TestMethod]
        public void VerifyArgumentNullException()
        {
            string[] numbers = { "first", "second", "third" };

            var parent = Container;
            parent.RegisterInstance("1", numbers[0])
                .RegisterInstance("2", numbers[1]);
            var child = parent.CreateChildContainer()
                .RegisterInstance("3", numbers[2]);

            List<string> nums = new List<string>(child.ResolveAll<string>());

            CollectionAssert.AreEquivalent(numbers, nums);
        }


        [TestMethod("Changes in parent array reflects in child")]
        public void ChangesInParentArray()
        {
            string[] numbers = { "first", "second", "third", "fourth" };
            var parent = Container;

            parent.RegisterInstance(numbers[0], "1")
                  .RegisterInstance(numbers[1], "2");
            var child = parent.CreateChildContainer();

            List<string> childnums = new List<string>(child.ResolveAll<string>());
            List<string> parentnums = new List<string>(parent.ResolveAll<string>());

            CollectionAssert.AreEquivalent(childnums, parentnums);

            parent.RegisterInstance(numbers[3], "4"); //Register an instance in Parent but not in child

            List<string> childnums2 = new List<string>(child.ResolveAll<string>());
            List<string> parentnums2 = new List<string>(parent.ResolveAll<string>());

            CollectionAssert.AreEquivalent(childnums2, parentnums2); //Both parent child should have same instances
        }


        [TestMethod]
        public void CreateParentChildContainersWithSameName()
        {
            var parent = Container;

            parent.RegisterType<ITemporary, Temp>("First");
            parent = parent.CreateChildContainer();
            parent.RegisterType<ITemporary, Temp>("First");

            List<ITemporary> count = new List<ITemporary>(parent.ResolveAll<ITemporary>());

            Assert.AreEqual(1, count.Count);
        }

        [TestMethod]
        public void MoreChildContainers1()
        {
            var parent = Container;

            parent.RegisterType<ITemporary, Temp>("First");
            parent.RegisterType<ITemporary, Temp>("First");

            var child1 = parent.CreateChildContainer();
            child1.RegisterType<ITemporary, Temp>("First");
            child1.RegisterType<ITemporary, Temp>("First");

            var child2 = child1.CreateChildContainer();
            child2.RegisterType<ITemporary, Temp>("First");
            child2.RegisterType<ITemporary, Temp>("First");

            var child3 = child2.CreateChildContainer();
            child3.RegisterType<ITemporary, Temp>("First");
            child3.RegisterType<ITemporary, Temp>("First");

            var child4 = child3.CreateChildContainer();
            child4.RegisterType<ITemporary, Temp>("First");

            ITemporary first = child4.Resolve<ITemporary>("First");

            child4.RegisterType<ITemporary, Temp>("First", new ContainerControlledLifetimeManager());
            var count = new List<ITemporary>(child4.ResolveAll<ITemporary>());

            Assert.AreEqual(1, count.Count);
        }

        [TestMethod]
        public void MoreChildContainers2()
        {
            var parent = Container;
            parent.RegisterType<ITemporary, Temp>("First", new HierarchicalLifetimeManager());
            var result  = parent.Resolve<ITemporary>("First");

            var child1 = parent.CreateChildContainer();
            child1.RegisterType<ITemporary, Temp>("First", new HierarchicalLifetimeManager());
            var result1 = child1.Resolve<ITemporary>("First");

            var child2 = child1.CreateChildContainer();
            child2.RegisterType<ITemporary, Temp>("First", new HierarchicalLifetimeManager());
            var result2 = child2.Resolve<ITemporary>("First");

            Assert.AreNotEqual(result.GetHashCode(), result1.GetHashCode());
            Assert.AreNotEqual(result.GetHashCode(), result2.GetHashCode());
            Assert.AreNotEqual(result1.GetHashCode(), result2.GetHashCode());

            List<ITemporary> count = new List<ITemporary>(child2.ResolveAll<ITemporary>());

            Assert.AreEqual(1, count.Count);
        }

        [TestMethod]
        public void GetObjectAfterDispose()
        {
            var parent = Container;
            parent.RegisterType<Temp>("First", new ContainerControlledLifetimeManager());

            var child = parent.CreateChildContainer();
            child.RegisterType<ITemporary>("First", new ContainerControlledLifetimeManager());
            parent.Dispose();
            Assert.ThrowsException<ResolutionFailedException>(() => child.Resolve<ITemporary>("First"));
        }

        [TestMethod]
        public void VerifyArgumentNotNullOrEmpty()
        {
            string[] numbers = { "first", "second", "third" };

            var parent = Container;
            parent.RegisterInstance("1", numbers[0])
                .RegisterInstance("2", numbers[1]);
            var child = parent.CreateChildContainer()
                .RegisterInstance("3", numbers[2]);
            List<string> nums = new List<string>(child.ResolveAll<string>());

            CollectionAssert.AreEquivalent(numbers, nums);
        }

        [TestMethod]
        public void VerifyArgumentNotNullOrEmpty1()
        {
            string[] numbers = { "first", "second", "third" };

            var parent = Container;
            parent.RegisterInstance("1", numbers[0])
                .RegisterInstance("2", numbers[1]);
            var child = parent.CreateChildContainer()
                .RegisterInstance("3", numbers[2]);
            List<string> nums = new List<string>(child.ResolveAll<string>());

            CollectionAssert.AreEquivalent(numbers, nums);
        }

        [TestMethod]
        public void VerifyArgumentNotNullOrEmpty2()
        {
            string[] numbers = { "first", "second", "third" };

            var parent = Container;
            parent.RegisterInstance("1", numbers[0])
                .RegisterInstance("2", numbers[1]);
            var child = parent.CreateChildContainer()
                .RegisterInstance("3", numbers[2]);
            List<string> nums = new List<string>(child.ResolveAll<string>());

            CollectionAssert.AreEquivalent(numbers, nums);
        }

        //bug # 3978 http://unity.codeplex.com/WorkItem/View.aspx?WorkItemId=6053
        [TestMethod]
        public void ChildParentRegisrationOverlapTest()
        {
            Container.RegisterInstance("str1", "string1");
            Container.RegisterInstance("str2", "string2");

            var child = Container.CreateChildContainer();

            child.RegisterInstance("str2", "string20");
            child.RegisterInstance("str3", "string30");

            var childStrSet = new HashSet<string>(child.ResolveAll<string>());
            var parentStrSet = new HashSet<string>( Container.ResolveAll<string>());

            Assert.IsTrue(childStrSet.SetEquals(new[] { "string1", "string20", "string30" }));
            Assert.IsTrue(parentStrSet.SetEquals(new[] { "string1", "string2" }));
        }
    }
}
