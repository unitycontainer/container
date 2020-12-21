using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;

namespace Container.Defaults
{

    [TestClass]
    public class PoliciesTests
    {
        Unity.Container.Defaults Defaults;
        object Instance = new object();
        private static Type[] TestTypes;
        private int DefaultPolicies = 6;

        [ClassInitialize]
        public static void InitializeClass(TestContext _)
        {
            TestTypes = Assembly.GetAssembly(typeof(int))
                                .DefinedTypes
                                .Where(t => t != typeof(IServiceProvider))
                                .Take(1500)
                                .ToArray();
        }


        [TestInitialize]
        public void InitializeTest() => Defaults = new Unity.Container.Defaults();

        [TestMethod]
        public void Baseline()
        {
            Assert.AreEqual(DefaultPolicies, Defaults.Span.Length);
        }

        [TestMethod]
        public void List_Null_Object_Value()
        {
            // Act
            Defaults.Set(null, typeof(object), Instance);

            // Validate
            var entry = Defaults.Span[DefaultPolicies];

            Assert.IsNull(entry.Target);
            Assert.AreEqual(typeof(object), entry.Type);
            Assert.AreSame(Instance, entry.Value);
        }

        [TestMethod]
        public void List_Replace()
        {
            var other = new object();

            // Act
            Defaults.Set(typeof(object), typeof(object), Instance);
            Defaults.Set(typeof(object), typeof(object), other);

            // Validate
            var entry = Defaults.Span[DefaultPolicies];

            Assert.AreEqual(1 + DefaultPolicies, Defaults.Span.Length);
            Assert.AreEqual(typeof(object), entry.Target);
            Assert.AreEqual(typeof(object), entry.Type);
            Assert.AreSame(other, entry.Value);
        }

        [TestMethod]
        public void Set_Replace()
        {
            var other = new object();

            // Act
            Defaults.Set(typeof(object), Instance);
            Defaults.Set(typeof(object), other);

            // Validate
            var entry = Defaults.Span[DefaultPolicies];

            Assert.AreEqual(1 + DefaultPolicies, Defaults.Span.Length);
            Assert.IsNull(entry.Target);
            Assert.AreEqual(typeof(object), entry.Type);
            Assert.AreSame(other, entry.Value);
        }

        [TestMethod]
        public void Set_Replace_List()
        {
            var other = new object();

            // Act
            Defaults.Set(null, typeof(object), Instance);
            Defaults.Set(typeof(object), other);

            // Validate
            var entry = Defaults.Span[DefaultPolicies];

            Assert.AreEqual(1 + DefaultPolicies, Defaults.Span.Length);
            Assert.IsNull(entry.Target);
            Assert.AreEqual(typeof(object), entry.Type);
            Assert.AreSame(other, entry.Value);
        }

        [TestMethod]
        public void Set_Event()
        {
            // Arrange
            object _value = null;
            var other = new object();

            // Act
            Defaults.Set(null, typeof(object), Instance, OnChange);
            Defaults.Set(null, typeof(object), other);

            // Validate
            Assert.AreSame(other, _value);

            void OnChange(object value) => _value = value;
        }

        [TestMethod]
        public void Set_Object_Value()
        {
            // Act
            Defaults.Set(typeof(object), Instance);

            // Validate
            var entry = Defaults.Span[DefaultPolicies];

            Assert.IsNull(entry.Target);
            Assert.AreEqual(typeof(object), entry.Type);
            Assert.AreSame(Instance, entry.Value);
        }

        [TestMethod]
        public void List_Different_TypeObject()
        {
            // Act
            Defaults.Set(typeof(Type), typeof(object), Instance);
            Defaults.Set(typeof(object), typeof(object), Instance);

            // Validate
            var span = Defaults.Span;

            Assert.AreEqual(2 + DefaultPolicies, span.Length);
        }

        [TestMethod]
        public void List_Different_ObjectType()
        {
            // Act
            Defaults.Set(typeof(object), typeof(Type),   Instance);
            Defaults.Set(typeof(object), typeof(object), Instance);

            // Validate
            var span = Defaults.Span;

            Assert.AreEqual(2 + DefaultPolicies, span.Length);
        }

        [TestMethod]
        public void Set_Different_TypeObject()
        {
            // Act
            Defaults.Set(typeof(Type), Instance);
            Defaults.Set(typeof(object), Instance);

            // Validate
            var span = Defaults.Span;

            Assert.AreEqual(2 + DefaultPolicies, span.Length);
        }

        [TestMethod]
        public void List_Overload_TypeObject()
        {
            // Act
            foreach (var type in TestTypes)
            { 
                Defaults.Set(type, typeof(object), Instance);
            }

            // Validate
            var span = Defaults.Span;

            Assert.AreEqual(1500 + DefaultPolicies, span.Length);
        }

        [TestMethod]
        public void List_Overload_ObjectType()
        {
            // Act
            foreach (var type in TestTypes)
            {
                Defaults.Set(typeof(object), type,  Instance);
            }

            foreach (var type in TestTypes)
            {
                Defaults.Set(typeof(string), type, Instance);
            }

            // Validate
            var span = Defaults.Span;

            Assert.AreEqual(3000 + DefaultPolicies, span.Length);
        }

        [TestMethod]
        public void Set_Overload()
        {
            // Act
            foreach (var type in TestTypes)
            {
                Defaults.Set(type, Instance);
            }

            // Validate
            var span = Defaults.Span;

            Assert.AreEqual(1500 + DefaultPolicies, span.Length);
        }


        [TestMethod]
        public void List_Get()
        {

            // Act
            foreach (var type in TestTypes)
            {
                Defaults.Set(type, typeof(object), Instance);
            }

            // Validate
            foreach (var type in TestTypes)
            {

                var value = Defaults.Get(type, typeof(object));
                Assert.AreSame(Instance, value);
            }
        }

        [TestMethod]
        public void List_Get_Null()
        {
            // Act
            foreach (var type in TestTypes.Take(88))
            {
                Defaults.Set(type, Instance);
            }

            // Validate
            foreach (var type in TestTypes.Take(88))
            {
                var value = Defaults.Get(null, type);
                Assert.AreSame(Instance, value);
            }
            
            Assert.IsNull(Defaults.Get(typeof(object), typeof(object)));
        }

        [TestMethod]
        public void Set_Get()
        {
            // Act
            foreach (var type in TestTypes)
            {
                Defaults.Set(null, type, Instance);
            }

            // Validate
            foreach (var type in TestTypes)
            {
                var value = Defaults.Get(type);
                Assert.AreSame(Instance, value);
            }
        }
    }
}
