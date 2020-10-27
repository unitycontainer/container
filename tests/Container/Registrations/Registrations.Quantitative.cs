using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Unity;

namespace Container.Registrations
{
    public partial class Registrations
    {
        [TestMethod]
        public void ToArray()
        {
            var registrations = Container.Registrations.ToArray();

            Assert.AreEqual(3, registrations.Length);
        }

        [TestMethod]
        public void ToArray_AlwaysTheSame()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(Container.Registrations.ToArray(), 
                                                   Container.Registrations.ToArray()));
        }

        [TestMethod]
        public void UnnamedFirst()
        {
            Container.RegisterInstance(Name, new object())
                     .RegisterInstance(new object());

            var list = Container.Registrations.ToList();
            var namedIndex   = list.FindIndex(r => typeof(object) == r.RegisteredType && null != r.Name);
            var unnamedIndex = list.FindIndex(r => typeof(object) == r.RegisteredType && null == r.Name);

            Assert.AreEqual(5, list.Count);
            Assert.IsTrue(unnamedIndex < namedIndex);
        }

        [TestMethod]
        public void UnnamedAllShowUp()
        {
            Container.RegisterInstance(new object())
                     .RegisterInstance(new object())
                     .RegisterInstance(new object());

            var list = Container.Registrations.ToList();

            Assert.AreEqual(6, list.Count);
        }

        [TestMethod]
        public void UnnamedFirstInHierarchies()
        {
            Container.RegisterInstance(Name, new object())
                     .RegisterInstance(new object());

            var child = Container.CreateChildContainer();
            child.RegisterInstance(Name, new object())
                 .RegisterInstance(new object())
                 .RegisterInstance(new object())
                 .RegisterInstance(new object());

            var list = child.Registrations.ToList();
            var namedIndex = list.FindIndex(r => typeof(object) == r.RegisteredType && null != r.Name);

            Assert.AreEqual(8, list.Count);
            Assert.IsTrue(7 == namedIndex);
        }

        [TestMethod]
        public void NamedShowUpInHierarchies()
        {
            object o1 = new object();
            object o2 = new object();

            Container
                .RegisterInstance<object>("o1", o1)
                .RegisterInstance<object>(o1)
                .RegisterInstance<object>("o2", o2);

            var obj = "child";
            var child = Container.CreateChildContainer()
                .RegisterInstance<object>(obj)
                .RegisterInstance<object>(o1)
                .RegisterInstance<object>("o2", obj);

            var results = Container.Registrations.ToList();
            var results1 = child.Registrations.ToList();

            Assert.AreEqual(6, results.Count);
            Assert.AreEqual(8, results1.Count);
        }


        [TestMethod]
        public void CacheOnUpdate()
        {
            // Act
            var enum1 = Container.Registrations.ToArray();
            Container.RegisterInstance(this);
            var enum2 = Container.Registrations.ToArray();

            // Validate
            Assert.AreNotSame(enum1, enum2);
            Assert.IsFalse(enum1.SequenceEqual(enum2));
        }

        [TestMethod]
        public void CacheOnParentUpdate()
        {
            // Arrange
            var child = ((IUnityContainer)Container).CreateChildContainer()
                                                    .CreateChildContainer();
            var enum1 = child.Registrations.ToArray();
            var enum2 = child.Registrations.ToArray();

            // Act
            Container.RegisterInstance(this);
            var enum3 = child.Registrations.ToArray();

            // Validate
            Assert.AreNotSame(enum1, enum3);
            Assert.IsTrue(enum1.SequenceEqual(enum2));
            Assert.IsFalse(enum1.SequenceEqual(enum3));
        }

        [TestMethod]
        public void Recreated()
        {
            // Arrange
            var enum1 = GetEnumeratorId();

            // Act
            GC.Collect(1, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            // Validate
            var enum2 = GetEnumeratorId();

            Assert.AreNotEqual(enum1, enum2);
        }

        [TestMethod]
        public void RegisterDynamic()
        {
            // Arrange
            Container.Register(AllRegistrations);
            
            // Act
            var array = Container.Registrations.ToArray();
            
            // Validate
            Assert.AreEqual(5998, array.Length);
        }

        [TestMethod]
        public void RegisterPreallocated()
        {
            // Arrange
            Container.Register(AllRegistrations);

            // Act
            var array = Container.Registrations.ToArray();

            // Validate
            Assert.AreEqual(5998, array.Length);
        }
    }
}
