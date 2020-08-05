using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Storage;

namespace Storage.Tests
{
    [TestClass]
    public class PolicyListTests
    {
        TestPolicyList PolicyList;
        object instance1 = new object();
        object instance2 = new object();
        object instance3 = new object();
        object instance4 = new object();
        object instance5 = new object();
        object instance6 = new object();
        object instance7 = new object();

        [TestInitialize]
        public void InitializeTest() => PolicyList = new TestPolicyList();


        [TestMethod]
        public void ClearNullTest() => PolicyList.Clear(null, typeof(PolicyListTests));

        [TestMethod]
        public void GetEmptyTest()
        {
            // Validate
            Assert.IsNull(PolicyList.Get(typeof(PolicyListTests), typeof(PolicyListTests)));
            Assert.AreEqual(0, PolicyList.Count);
        }

        [TestMethod]
        public void SetGetNullTest()
        {
            // Act
            PolicyList.Set(null, typeof(PolicyListTests), instance1);
            var instance = PolicyList.Get(null, typeof(PolicyListTests));

            // Validate
            Assert.AreSame(instance1, instance);
            Assert.AreEqual(1, PolicyList.Count);
        }

        [TestMethod]
        public void SetClearGetNullTest()
        {
            // Act
            PolicyList.Set(null, typeof(PolicyListTests), instance1);
            PolicyList.Clear(null, typeof(PolicyListTests));

            // Validate
            Assert.IsNull(PolicyList.Get(null, typeof(PolicyListTests)));
            Assert.AreEqual(1, PolicyList.Count);
        }

        [TestMethod]
        public void SetGetTest()
        {
            // Act
            PolicyList.Set(typeof(List<string>), typeof(List<string>), this);
            PolicyList.Set(typeof(PolicyListTests), typeof(PolicyListTests), instance1);
            var instance = PolicyList.Get(typeof(PolicyListTests), typeof(PolicyListTests));

            // Validate
            Assert.AreSame(instance1, instance);
            Assert.AreEqual(2, PolicyList.Count);
        }

        [TestMethod]
        public void SetClearGetTest()
        {
            // Act
            PolicyList.Set(typeof(PolicyListTests), typeof(PolicyListTests), instance1);
            PolicyList.Clear(typeof(PolicyListTests), typeof(PolicyListTests));

            // Validate
            Assert.IsNull(PolicyList.Get(typeof(PolicyListTests), typeof(PolicyListTests)));
            Assert.AreEqual(1, PolicyList.Count);
        }

        [TestMethod]
        public void ExpandTest()
        {
            // Arrange
            var instance1 = new object();
            var instance2 = new object();
            var instance3 = new object();
            var instance4 = new object();
            var instance5 = new object();
            var instance6 = new object();
            var instance7 = new object();

            // Act
            PolicyList.Set(null, typeof(List<int>),    instance1);
            PolicyList.Set(null, typeof(List<char>),   instance2);
            // Expand here
            PolicyList.Set(null, typeof(List<string>), instance3);
            PolicyList.Set(null, typeof(List<bool>),   instance4);
            PolicyList.Set(null, typeof(List<long>),   instance5);
            PolicyList.Set(null, typeof(List<object>), instance6);
            // Expand here
            PolicyList.Set(null, typeof(List<uint>),   instance7);

            // Validate
            Assert.AreSame(instance1, PolicyList.Get(null, typeof(List<int>)));
            Assert.AreSame(instance2, PolicyList.Get(null, typeof(List<char>)));
            Assert.AreSame(instance3, PolicyList.Get(null, typeof(List<string>)));
            Assert.AreSame(instance4, PolicyList.Get(null, typeof(List<bool>)));
            Assert.AreSame(instance5, PolicyList.Get(null, typeof(List<long>)));
            Assert.AreSame(instance6, PolicyList.Get(null, typeof(List<object>)));
            Assert.AreSame(instance7, PolicyList.Get(null, typeof(List<uint>)));
            Assert.AreEqual(7, PolicyList.Count);
        }

        [TestMethod]
        public void ReplaceTest()
        {
            // Act
            PolicyList.Set(typeof(List<object>), typeof(List<int>),    instance1);
            PolicyList.Set(typeof(List<object>), typeof(List<char>),   instance2);
            PolicyList.Set(typeof(List<object>), typeof(List<string>), instance3);
            PolicyList.Set(typeof(List<object>), typeof(List<bool>),   instance4);
            PolicyList.Set(typeof(List<object>), typeof(List<long>),   instance5);
            PolicyList.Set(typeof(List<object>), typeof(List<object>), instance6);
            PolicyList.Set(typeof(List<object>), typeof(List<bool>),   instance7);

            // Validate
            Assert.AreSame(instance1, PolicyList.Get(typeof(List<object>), typeof(List<int>)));
            Assert.AreSame(instance2, PolicyList.Get(typeof(List<object>), typeof(List<char>)));
            Assert.AreSame(instance3, PolicyList.Get(typeof(List<object>), typeof(List<string>)));
            Assert.AreSame(instance7, PolicyList.Get(typeof(List<object>), typeof(List<bool>)));
            Assert.AreSame(instance5, PolicyList.Get(typeof(List<object>), typeof(List<long>)));
            Assert.AreSame(instance6, PolicyList.Get(typeof(List<object>), typeof(List<object>)));
            Assert.AreEqual(6, PolicyList.Count);
        }

        [TestMethod]
        public void CollisionTest()
        {
            // Arrange
            PolicyList = new TestPolicyList() { Code = 642198413 };

            // Act
            PolicyList.Set(typeof(List<object>), typeof(List<int>),    instance1);
            PolicyList.Set(typeof(List<object>), typeof(List<char>),   instance2);
            PolicyList.Set(typeof(List<object>), typeof(List<string>), instance3);
            // Missing
            PolicyList.Set(typeof(List<object>), typeof(List<long>),   instance4);
            PolicyList.Set(typeof(List<long>),   typeof(List<string>), instance5);

            // Validate
            Assert.AreSame(instance1, PolicyList.Get(typeof(List<object>), typeof(List<int>)));
            Assert.AreSame(instance2, PolicyList.Get(typeof(List<object>), typeof(List<char>)));
            Assert.AreSame(instance3, PolicyList.Get(typeof(List<object>), typeof(List<string>)));
            // Missing
            Assert.IsNull(PolicyList.Get(typeof(List<object>), typeof(List<object>)));
            Assert.IsNull(PolicyList.Get(typeof(List<string>), typeof(List<string>)));
            Assert.AreEqual(5, PolicyList.Count);
        }


        [TestMethod]
        public void CollisionClearTest()
        {
            // Arrange
            PolicyList = new TestPolicyList() { Code = 642198413 };
            PolicyList.Set(typeof(List<object>), typeof(List<int>), instance1);
            PolicyList.Set(typeof(List<object>), typeof(List<char>), instance2);
            PolicyList.Set(typeof(List<object>), typeof(List<string>), instance3);

            // Act
            PolicyList.Clear(typeof(List<object>), typeof(List<int>)   );
            PolicyList.Clear(typeof(List<object>), typeof(List<char>)  );
            PolicyList.Clear(typeof(List<object>), typeof(List<string>));
            // Missing
            PolicyList.Clear(typeof(List<object>), typeof(List<object>));
            PolicyList.Clear(typeof(List<string>), typeof(List<object>));

            // Validate
            Assert.IsNull(PolicyList.Get(typeof(List<object>), typeof(List<int>)));
            Assert.IsNull(PolicyList.Get(typeof(List<object>), typeof(List<char>)));
            Assert.IsNull(PolicyList.Get(typeof(List<object>), typeof(List<string>)));
            Assert.AreEqual(3, PolicyList.Count);
        }

        #region Test Data

        [DebuggerDisplay("{Name}")]
        public class TestPolicyList : PolicyList<object>
        {
            public TestPolicyList() : base(0) { }

            public int Count => _count;

            public int Code { get; set; }

            protected override int HashCode(Type type, Type policy) 
                => 0 == Code ? base.HashCode(type, policy) : Code;
        }

        #endregion
    }
}
