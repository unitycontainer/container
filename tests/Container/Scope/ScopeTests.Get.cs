using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity;

namespace Container.Scopes
{
    public partial class ScopeTests
    {
        [TestMethod]
        public void GetEmptyTest()
        {
            // Arrange
            var type = Manager.GetType();

            // Validate
            Assert.IsNull(Scope.Get(new Contract( type, type.Name)));
        }

        [TestMethod]
        public void GetTest()
        {
            // Arrange
            Scope.BuiltIn(typeof(List<>), Manager);

            // Act
            var manager = Scope.Get(new Contract(typeof(List<>)));

            // Validate
            Assert.AreSame(Manager, manager);
        }

        [TestMethod]
        public void GetFactoryTest()
        {
            // Arrange
            Scope.BuiltIn(typeof(List<>), Manager);

            // Act
            var manager1 = Scope.GetBoundGeneric(new Contract(typeof(List<int>)), new Contract(typeof(List<>)));
            var manager2 = Scope.GetBoundGeneric(new Contract(typeof(List<string>)), new Contract(typeof(List<>)));

            // Validate
            Assert.AreNotSame(Manager,  manager1);
            Assert.AreNotSame(Manager,  manager2);
            Assert.AreNotSame(manager1, manager2);

            Assert.AreEqual(Manager.GetType(),  manager1.GetType());
            Assert.AreEqual(manager1.GetType(), manager2.GetType());
        }

        [TestMethod]
        public void GetFactoryFromThreads()
        {
            object manager1 = null;
            object manager2 = null;
            object manager3 = null;
            object manager4 = null;

            // Arrange
            Scope.BuiltIn(typeof(List<>), Manager);
            var sync = new ManualResetEvent(false);


            Thread thread1 = new Thread(delegate ()
            {
                sync.WaitOne();
                manager1 = Scope.GetBoundGeneric(new Contract(typeof(List<int>)), new Contract(typeof(List<>)));
            })
            { Name = "1"};

            Thread thread2 = new Thread(delegate ()
            {
                sync.WaitOne();
                manager2 = Scope.GetBoundGeneric(new Contract(typeof(List<int>)), new Contract(typeof(List<>)));
            })
            { Name = "2" };

            Thread thread3 = new Thread(delegate ()
            {
                sync.WaitOne();
                manager3 = Scope.GetBoundGeneric(new Contract(typeof(List<int>)), new Contract(typeof(List<>)));
            })
            { Name = "3" };

            Thread thread4 = new Thread(delegate ()
            {
                sync.WaitOne();
                manager4 = Scope.GetBoundGeneric(new Contract(typeof(List<int>)), new Contract(typeof(List<>)));
            })
            { Name = "4" };

            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();

            Thread.Sleep(200);
            sync.Set();

            thread1.Join();
            thread2.Join();
            thread3.Join();
            thread4.Join();

            Assert.AreSame(manager1, manager2);
            Assert.AreSame(manager3, manager2);
            Assert.AreSame(manager3, manager4);
        }

        [TestMethod]
        public void GetFactoryFromThreadsNamed()
        {
            object manager1 = null;
            object manager2 = null;
            object manager3 = null;
            object manager4 = null;
            ReadOnlySpan<RegistrationDescriptor> span = new RegistrationDescriptor[] 
            { 
                new RegistrationDescriptor(Name, Manager, typeof(List<>)) 
            };


            // Arrange
            Scope.Register(in span);
            var sync = new ManualResetEvent(false);


            Thread thread1 = new Thread(delegate ()
            {
                sync.WaitOne();
                manager1 = Scope.GetBoundGeneric(new Contract(typeof(List<int>), Name), new Contract(typeof(List<>), Name));
            })
            { Name = "1" };

            Thread thread2 = new Thread(delegate ()
            {
                sync.WaitOne();
                manager2 = Scope.GetBoundGeneric(new Contract(typeof(List<int>), Name), new Contract(typeof(List<>), Name));
            })
            { Name = "2" };

            Thread thread3 = new Thread(delegate ()
            {
                sync.WaitOne();
                manager3 = Scope.GetBoundGeneric(new Contract(typeof(List<int>), Name), new Contract(typeof(List<>), Name));
            })
            { Name = "3" };

            Thread thread4 = new Thread(delegate ()
            {
                sync.WaitOne();
                manager4 = Scope.GetBoundGeneric(new Contract(typeof(List<int>), Name), new Contract(typeof(List<>), Name));
            })
            { Name = "4" };

            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();

            Thread.Sleep(200);
            sync.Set();

            thread1.Join();
            thread2.Join();
            thread3.Join();
            thread4.Join();

            Assert.AreSame(manager1, manager2);
            Assert.AreSame(manager3, manager2);
            Assert.AreSame(manager3, manager4);
        }

        [TestMethod]
        public void GetFactoriesFromThreads()
        {
            object manager1 = null;
            object manager2 = null;
            object manager3 = null;
            object manager4 = null;

            // Arrange
            Scope.BuiltIn(typeof(List<>), Manager);
            var sync = new ManualResetEvent(false);


            Thread thread1 = new Thread(delegate ()
            {
                sync.WaitOne();
                manager1 = Scope.GetBoundGeneric(new Contract(typeof(List<int>)), new Contract(typeof(List<>)));
            })
            { Name = "1" };

            Thread thread2 = new Thread(delegate ()
            {
                sync.WaitOne();
                manager2 = Scope.GetBoundGeneric(new Contract(typeof(List<long>)), new Contract(typeof(List<>)));
            })
            { Name = "2" };

            Thread thread3 = new Thread(delegate ()
            {
                sync.WaitOne();
                manager3 = Scope.GetBoundGeneric(new Contract(typeof(List<string>)), new Contract(typeof(List<>)));
            })
            { Name = "3" };

            Thread thread4 = new Thread(delegate ()
            {
                sync.WaitOne();
                manager4 = Scope.GetBoundGeneric(new Contract(typeof(List<object>)), new Contract(typeof(List<>)));
            })
            { Name = "4" };

            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();

            Thread.Sleep(200);
            sync.Set();

            thread1.Join();
            thread2.Join();
            thread3.Join();
            thread4.Join();

            Assert.AreNotSame(manager1, manager2);
            Assert.AreNotSame(manager1, manager3);
            Assert.AreNotSame(manager1, manager4);
            Assert.AreNotSame(manager2, manager3);
            Assert.AreNotSame(manager2, manager4);
            Assert.AreNotSame(manager3, manager4);
        }

        [TestMethod]
        public void GetFactoriesFromThreadsNamed()
        {
            object manager1 = null;
            object manager2 = null;
            object manager3 = null;
            object manager4 = null;
            ReadOnlySpan<RegistrationDescriptor> span = new RegistrationDescriptor[]
            {
                new RegistrationDescriptor(Name, Manager, typeof(List<>))
            };


            // Arrange
            Scope.Register(in span);
            var sync = new ManualResetEvent(false);


            Thread thread1 = new Thread(delegate ()
            {
                sync.WaitOne();
                manager1 = Scope.GetBoundGeneric(new Contract(typeof(List<int>), Name), new Contract(typeof(List<>), Name));
            })
            { Name = "1" };

            Thread thread2 = new Thread(delegate ()
            {
                sync.WaitOne();
                manager2 = Scope.GetBoundGeneric(new Contract(typeof(List<long>), Name), new Contract(typeof(List<>), Name));
            })
            { Name = "2" };

            Thread thread3 = new Thread(delegate ()
            {
                sync.WaitOne();
                manager3 = Scope.GetBoundGeneric(new Contract(typeof(List<string>), Name), new Contract(typeof(List<>), Name));
            })
            { Name = "3" };

            Thread thread4 = new Thread(delegate ()
            {
                sync.WaitOne();
                manager4 = Scope.GetBoundGeneric(new Contract(typeof(List<object>), Name), new Contract(typeof(List<>), Name));
            })
            { Name = "4" };

            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();

            Thread.Sleep(200);
            sync.Set();

            thread1.Join();
            thread2.Join();
            thread3.Join();
            thread4.Join();

            Assert.AreNotSame(manager1, manager2);
            Assert.AreNotSame(manager1, manager3);
            Assert.AreNotSame(manager1, manager4);
            Assert.AreNotSame(manager2, manager3);
            Assert.AreNotSame(manager2, manager4);
            Assert.AreNotSame(manager3, manager4);
        }
    }
}
