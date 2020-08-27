using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Unity.Lifetime;

namespace Lifetime.Managers
{
    [TestClass]
    public class PerThread : LifetimeManagerTests
    {
        private object TestObject1;
        private object TestObject2;

        protected override LifetimeManager GetManager() => new PerThreadLifetimeManager();

        [TestInitialize]
        public override void SetupTest()
        {
            base.SetupTest();

            TestObject1 = new object();
            TestObject2 = new object();
        }

        [TestMethod]
        public override void TryGetValueTest()
        {
            Thread thread1 = new Thread(delegate ()
            {
                Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(LifetimeContainer));

                // Act
                TestManager.SetValue(TestObject1, LifetimeContainer);

                // Validate
                Assert.AreSame(TestObject1, TestManager.TryGetValue(LifetimeContainer));
            })
            { Name = "1" };

            Thread thread2 = new Thread(delegate ()
            {
                Assert.AreSame(LifetimeManager.NoValue, TestManager.TryGetValue(LifetimeContainer));

                // Act
                TestManager.SetValue(TestObject2, LifetimeContainer);

                // Validate
                Assert.AreSame(TestObject2, TestManager.TryGetValue(LifetimeContainer));
            })
            { Name = "2" };

            thread1.Start();
            thread2.Start();

            thread2.Join();
            thread1.Join();

            base.TryGetValueTest();
        }

        [TestMethod]
        public override void GetValueTest()
        {
            Thread thread1 = new Thread(delegate ()
            {
                Assert.AreSame(LifetimeManager.NoValue, TestManager.GetValue(LifetimeContainer));

                // Act
                TestManager.SetValue(TestObject1, LifetimeContainer);

                // Validate
                Assert.AreSame(TestObject1, TestManager.GetValue(LifetimeContainer));
            })
            { Name = "1" };

            Thread thread2 = new Thread(delegate ()
            {
                Assert.AreSame(LifetimeManager.NoValue, TestManager.GetValue(LifetimeContainer));

                // Act
                TestManager.SetValue(TestObject2, LifetimeContainer);

                // Validate
                Assert.AreSame(TestObject2, TestManager.GetValue(LifetimeContainer));
            })
            { Name = "2" };

            thread1.Start();
            thread2.Start();

            thread2.Join();
            thread1.Join();

            base.GetValueTest();
        }

        [TestMethod]
        public override void TryGetSetOtherContainerTest()
        {
            object value1 = null;
            object value2 = null;
            object value3 = null;
            object value4 = null;
            object value5 = null;
            object value6 = null;

            Thread thread1 = new Thread(delegate ()
            {
                value1 = TestManager.TryGetValue(LifetimeContainer);
                value2 = TestManager.GetValue(LifetimeContainer);

                // Act
                TestManager.SetValue(TestObject1, LifetimeContainer);

                // Validate
                value3 = TestManager.TryGetValue(LifetimeContainer);
                value4 = TestManager.GetValue(LifetimeContainer);

                value5 = TestManager.TryGetValue(OtherContainer);
                value6 = TestManager.GetValue(OtherContainer);
            });

            thread1.Start();
            thread1.Join();

            // Validate
            Assert.AreSame(LifetimeManager.NoValue, value1);
            Assert.AreSame(LifetimeManager.NoValue, value2);
            Assert.AreSame(TestObject1, value3);
            Assert.AreSame(TestObject1, value4);
            Assert.AreSame(TestObject1, value5);
            Assert.AreSame(TestObject1, value6);

            base.TryGetSetOtherContainerTest();
        }

        [TestMethod]
        public override void ValuesFromDifferentThreads()
        {
            TestManager.SetValue(TestObject, LifetimeContainer);

            object value1 = null;
            object value2 = null;

            Thread thread1 = new Thread(delegate ()
            {
                value1 = TestManager.TryGetValue(LifetimeContainer);
                value2 = TestManager.GetValue(LifetimeContainer);

            });

            thread1.Start();
            thread1.Join();

            Assert.AreSame(TestObject, TestManager.TryGetValue(LifetimeContainer));
            Assert.AreSame(TestObject, TestManager.GetValue(LifetimeContainer));

            Assert.AreSame(LifetimeManager.NoValue, value1);
            Assert.AreSame(LifetimeManager.NoValue, value2);
        }
    }
}
