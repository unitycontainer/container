using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using Unity.Storage;

namespace Storage.Tests
{
    [TestClass]
    public class QuickSetTests
    {
        int HashMask = unchecked((int)(uint.MaxValue >> 1));

        [TestMethod]
        public void Baseline()
        {
            // Arrange
            var set = new QuickSet<Type>();

            // Validate
            Assert.IsTrue(set.Add(null));
            Assert.IsTrue(set.Add(typeof(QuickSetTests)));
            Assert.IsFalse(set.Add(typeof(QuickSetTests)));
        }

        [TestMethod]
        public void SameHashCodeTest()
        {
            // Arrange
            var set = new QuickSet<SameHashCode>();
            var instance = new SameHashCode();

            // Validate
            Assert.IsTrue(set.Add(instance));
            Assert.IsTrue(set.Add(new SameHashCode()));
            Assert.IsTrue(set.Add(new SameHashCode()));
            Assert.IsTrue(set.Add(new SameHashCode()));
            Assert.IsTrue(set.Add(new SameHashCode()));
            Assert.IsTrue(set.Add(new SameHashCode { Code = instance.Code | (-1 ^ HashMask) } ));
            Assert.IsTrue(set.Add(new SameHashCode()));
            Assert.IsTrue(set.Add(new SameHashCode()));
            Assert.IsTrue(set.Add(new SameHashCode()));
            Assert.IsFalse(set.Add(instance));
            Assert.IsFalse(set.Add(instance));
            Assert.IsFalse(set.Add(instance));
            Assert.IsFalse(set.Add(instance));
        }

        [TestMethod]
        public void AddSameHashCodeTest()
        {
            // Arrange
            var set = new QuickSet<object>();
            var instance = new object();
            var code = instance.GetHashCode();
            var wrong = code | (-1 ^ HashMask);


            // Validate
            Assert.IsTrue(set.Add(instance,     code));
            Assert.IsTrue(set.Add(new object(), wrong));
            Assert.IsTrue(set.Add(new object(), code));
            Assert.IsTrue(set.Add(new object(), code));
            Assert.IsTrue(set.Add(new object(), code));
            Assert.IsTrue(set.Add(new object(), code));
            Assert.IsTrue(set.Add(new object(), code));
            Assert.IsTrue(set.Add(new object(), code));
            Assert.IsTrue(set.Add(new object(), code));
            Assert.IsTrue(set.Add(new object(), code));
            Assert.IsFalse(set.Add(instance,    code));
            Assert.IsFalse(set.Add(instance,    code));
            Assert.IsFalse(set.Add(instance,    code));
        }

        [DebuggerDisplay("{Name}")]
        public class SameHashCode
        {
            public string Name { get; } = Guid.NewGuid().ToString();
            public int Code { get; set; } = 753951;

            public override int GetHashCode() => Code;

            public override bool Equals(object obj)
            {
                if (obj is SameHashCode other && Name == other.Name) 
                    return true;
                
                return false;
            }
        }
    }
}
