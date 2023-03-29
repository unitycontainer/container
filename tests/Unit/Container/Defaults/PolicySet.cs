using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using Unity.Container.Tests;
using Unity.Policy;

namespace Container
{
    public partial class Defaults
    {
        #region Set

        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, nameof(IPolicySet))]
        [ExpectedException(typeof(NullReferenceException))]
        public void Set_null_instance()
        {
            // Act
            Policies.Set(null, Instance);
        }

        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, nameof(IPolicySet))]
        public void Set_type_instance()
        {
            // Act
            Policies.Set(typeof(object), Instance);

            // Validate
            var entry = Policies.Span[DefaultPolicies];

            Assert.IsNull(entry.Target);
            Assert.AreEqual(typeof(object), entry.Type);
            Assert.AreSame(Instance, entry.Value);
        }

        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, nameof(IPolicySet))]
        public void Set_type_replace()
        {
            var other = new object();

            // Act
            Policies.Set(typeof(object), Instance);
            Policies.Set(typeof(object), other);

            // Validate
            var entry = Policies.Span[DefaultPolicies];

            Assert.AreEqual(1 + DefaultPolicies, Policies.Span.Length);
            Assert.IsNull(entry.Target);
            Assert.AreEqual(typeof(object), entry.Type);
            Assert.AreSame(other, entry.Value);
        }


        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, nameof(IPolicySet))]
        public void Set_type_set2list()
        {
            var other = new object();

            // Act
            Policies.Set(typeof(object), Instance);
            Policies.Set(null, typeof(object), other);

            // Validate
            var entry = Policies.Span[DefaultPolicies];

            Assert.AreEqual(1 + DefaultPolicies, Policies.Span.Length);
            Assert.IsNull(entry.Target);
            Assert.AreEqual(typeof(object), entry.Type);
            Assert.AreSame(other, entry.Value);
        }


        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, nameof(IPolicySet))]
        public void Set_different_instance()
        {
            // Act
            Policies.Set(typeof(Type), Instance);
            Policies.Set(typeof(object), Instance);

            // Validate
            var span = Policies.Span;

            Assert.AreEqual(2 + DefaultPolicies, span.Length);
        }


        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, nameof(IPolicySet))]
        public void Set_overload_instance()
        {
            // Act
            foreach (var type in TestTypes)
            {
                Policies.Set(type, Instance);
            }

            // Validate
            var span = Policies.Span;

            Assert.AreEqual(1500 + DefaultPolicies, span.Length);
        }


        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, nameof(IPolicySet))]
        public void Set_overload_twice()
        {
            // Act
            foreach (var type in TestTypes)
            {
                Policies.Set(type, Instance);
            }

            foreach (var type in TestTypes)
            {
                Policies.Set(type, Instance);
            }

            // Validate
            var span = Policies.Span;

            Assert.AreEqual(1500 + DefaultPolicies, span.Length);
        }

        #endregion


        #region Get

        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, nameof(IPolicySet))]
        public void Get_type_instance()
        {
            // Act
            foreach (var type in TestTypes)
            {
                Policies.Set(type, Instance);
            }

            // Validate
            foreach (var type in TestTypes)
            {

                var value = Policies.Get(type);
                Assert.AreSame(Instance, value);
            }
        }


        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, nameof(IPolicySet))]
        public void Get_type_miss()
        {
            // Arrange
            foreach (var type in TestTypes)
                Policies.Set(typeof(object), type, Instance);

            foreach (var type in TestTypes.Take(TestTypes.Length - 10))
                Policies.Set(type, Instance);

            // Validate
            Assert.IsNull(Policies.Get(TestTypes[TestTypes.Length - 1]));
            Assert.IsNull(Policies.Get(TestTypes[TestTypes.Length - 2]));
            Assert.IsNull(Policies.Get(TestTypes[TestTypes.Length - 3]));
            Assert.IsNull(Policies.Get(TestTypes[TestTypes.Length - 4]));
            Assert.IsNull(Policies.Get(TestTypes[TestTypes.Length - 5]));
        }


        [PatternTestMethod(SET_POLICIES), TestProperty(INTERFACE, nameof(IPolicies))]
        public void Get_miss_handler()
        {
            Assert.IsNull(Policies.Get(typeof(object), OnPolicyChanged));
            Assert.IsNull(Target);
            Assert.IsNull(Type);
            Assert.IsNull(Policy);

            Policies.Set(typeof(object), Instance);

            Assert.IsNull(Target);
            Assert.IsNotNull(Type);
            Assert.IsNotNull(Policy);
            Assert.AreSame(Instance, Policy);
        }


        [PatternTestMethod(SET_POLICIES), TestProperty(INTERFACE, nameof(IPolicies))]
        public void Get_hit_handler()
        {
            foreach (var type in TestTypes) Policies.Set(typeof(object), type, Instance);
            foreach (var type in TestTypes.Take(TestTypes.Length - 10)) Policies.Set(type, Instance);

            Assert.AreSame(Instance, Policies.Get(TestTypes[10], OnPolicyChanged));
            Assert.IsNull(Target);
            Assert.IsNull(Type);
            Assert.IsNull(Policy);

            Policies.Clear(TestTypes[10]);

            Assert.IsNull(Target);
            Assert.AreEqual(TestTypes[10], Type);
            Assert.IsNull(Policy);
        }


        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, nameof(IPolicies))]
        public void Get_type_other()
        {
            using var barrier = new Barrier(2);
            object result = null;

            Thread thread1 = new Thread(delegate ()
            {
                lock (Policies.SyncObject)
                {
                    barrier.SignalAndWait();
                    Thread.Sleep(20);
                    Policies.Set(typeof(Type), Instance);
                }
            })
            { Name = "1" };

            Thread thread2 = new Thread(delegate ()
            {
                barrier.SignalAndWait();
                result = Policies.Get(typeof(object), OnPolicyChanged);
            })
            { Name = "2" };

            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            Assert.IsNull(result);
        }


        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, nameof(IPolicies))]
        public void Get_type_changed()
        {
            using var barrier = new Barrier(2);
            object result = null;

            Thread thread1 = new Thread(delegate ()
            {
                lock (Policies.SyncObject)
                {
                    barrier.SignalAndWait();
                    Thread.Sleep(20);
                    Policies.Set(typeof(object), Instance);
                }
            })
            { Name = "1" };

            Thread thread2 = new Thread(delegate ()
            {
                barrier.SignalAndWait();
                result = Policies.Get(typeof(object), OnPolicyChanged);
            })
            { Name = "2" };

            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            Assert.AreSame(Instance, result);
        }

        #endregion


        #region Clear

        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, nameof(IPolicySet))]
        public void Clear_type_hit()
        {
            foreach (var type in TestTypes) Policies.Set(type, Instance);
            foreach (var type in TestTypes) Policies.Clear(type);
            foreach (var type in TestTypes) Assert.IsNull(Policies.Get(type));
        }


        [PatternTestMethod(SET_PATTERN), TestProperty(INTERFACE, nameof(IPolicySet))]
        public void Clear_type_miss()
        {
            // Act
            Policies.Set(typeof(object), Instance);
            Policies.Clear(typeof(Type));
            Assert.IsNotNull(Policies.Get(typeof(object)));
        }

        #endregion


        #region Notification

        #endregion
    }
}
