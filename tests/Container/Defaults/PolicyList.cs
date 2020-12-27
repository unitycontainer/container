using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using Unity.Container.Tests;
using Unity.Extension;

namespace Container
{
    public partial class Defaults
    {
        #region Set

        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicyList))]
        [ExpectedException(typeof(NullReferenceException))]
        public void Set_null_null_instance()
        {
            // Act
            Policies.Set(null, null, Instance);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicyList))]
        [ExpectedException(typeof(NullReferenceException))]
        public void Set_target_null_instance()
        {
            // Act
            Policies.Set(typeof(object), null, Instance);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicyList))]
        public void Set_null_type_instance()
        {
            // Act
            Policies.Set(null, typeof(object), Instance);

            // Validate
            var entry = Policies.Span[DefaultPolicies];

            Assert.IsNull(entry.Target);
            Assert.AreEqual(typeof(object), entry.Type);
            Assert.AreSame(Instance, entry.Value);
        }

        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicyList))]
        public void Set_target_type_replace()
        {
            var other = new object();

            // Act
            Policies.Set(typeof(object), typeof(object), Instance);
            Policies.Set(typeof(object), typeof(object), other);

            // Validate
            var entry = Policies.Span[DefaultPolicies];

            Assert.AreEqual(1 + DefaultPolicies, Policies.Span.Length);
            Assert.AreEqual(typeof(object), entry.Target);
            Assert.AreEqual(typeof(object), entry.Type);
            Assert.AreSame(other, entry.Value);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicyList))]
        public void Set_target_type_list2set()
        {
            var other = new object();

            // Act
            Policies.Set(null, typeof(object), Instance);
            Policies.Set(typeof(object), other);

            // Validate
            var entry = Policies.Span[DefaultPolicies];

            Assert.AreEqual(1 + DefaultPolicies, Policies.Span.Length);
            Assert.IsNull(entry.Target);
            Assert.AreEqual(typeof(object), entry.Type);
            Assert.AreSame(other, entry.Value);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicyList))]
        public void Set_different_type_instance()
        {
            // Act
            Policies.Set(typeof(Type), typeof(object), Instance);
            Policies.Set(typeof(object), typeof(object), Instance);

            // Validate
            var span = Policies.Span;

            Assert.AreEqual(2 + DefaultPolicies, span.Length);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicyList))]
        public void Set_target_different_instance()
        {
            // Act
            Policies.Set(typeof(object), typeof(Type), Instance);
            Policies.Set(typeof(object), typeof(object), Instance);

            // Validate
            var span = Policies.Span;

            Assert.AreEqual(2 + DefaultPolicies, span.Length);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicyList))]
        public void Set_overload_type_instance()
        {
            // Act
            foreach (var type in TestTypes)
            {
                Policies.Set(type, typeof(object), Instance);
            }

            // Validate
            var span = Policies.Span;

            Assert.AreEqual(1500 + DefaultPolicies, span.Length);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicyList))]
        public void Set_target_overload_instance()
        {
            // Act
            foreach (var type in TestTypes)
            {
                Policies.Set(typeof(object), type, Instance);
            }

            foreach (var type in TestTypes)
            {
                Policies.Set(typeof(string), type, Instance);
            }

            // Validate
            var span = Policies.Span;

            Assert.AreEqual(3000 + DefaultPolicies, span.Length);
        }

        #endregion


        #region Get

        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicyList))]
        [ExpectedException(typeof(NullReferenceException))]
        public void Get_null_null_instance()
        {
            // Act
            _ = Policies.Get(null, (Type)null);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicyList))]
        [ExpectedException(typeof(NullReferenceException))]
        public void Get_target_null_instance()
        {
            // Act
            _ = Policies.Get(typeof(object), (Type)null);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicyList))]
        public void Get_null_type_instance()
        {
            // Act
            foreach (var type in TestTypes.Take(44))
            {
                Policies.Set(null, type, Instance);
            }

            // Validate
            foreach (var type in TestTypes.Take(44))
            {
                var value = Policies.Get(null, type);
                Assert.AreSame(Instance, value);
            }

            Assert.IsNull(Policies.Get(typeof(object), typeof(object)));
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicyList))]
        public void Get_same_type_instance()
        {

            // Act
            foreach (var type in TestTypes)
            {
                Policies.Set(typeof(object), type, Instance);
            }

            // Validate
            foreach (var type in TestTypes)
            {

                var value = Policies.Get(typeof(object), type);
                Assert.AreSame(Instance, value);
            }
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicyList))]
        public void Get_target_same_instance()
        {

            // Act
            foreach (var type in TestTypes)
            {
                Policies.Set(type, typeof(object), Instance);
            }

            // Validate
            foreach (var type in TestTypes)
            {

                var value = Policies.Get(type, typeof(object));
                Assert.AreSame(Instance, value);
            }
        }


        [PatternTestMethod(LIST_POLICIES), TestProperty(INTERFACE, nameof(IPolicies))]
        public void Get_null_miss_handler()
        {
            Assert.IsNull(Policies.Get(null, typeof(object), OnPolicyChanged));
            Assert.IsNull(Target);
            Assert.IsNull(Type);
            Assert.IsNull(Policy);

            Policies.Set(null, typeof(object), Instance);

            Assert.IsNull(Target);
            Assert.IsNotNull(Type);
            Assert.IsNotNull(Policy);
            Assert.AreSame(Instance, Policy);
        }


        [PatternTestMethod(LIST_POLICIES), TestProperty(INTERFACE, nameof(IPolicies))]
        public void Get_null_hit_handler()
        {
            object Other = new object();
            Policies.Set(typeof(object), Instance);

            Assert.AreSame(Instance, Policies.Get(null, typeof(object), OnPolicyChanged));
            Assert.IsNull(Target);
            Assert.IsNull(Type);
            Assert.IsNull(Policy);

            Policies.Set(typeof(object), Other);

            Assert.IsNull(Target);
            Assert.AreEqual(typeof(object), Type);
            Assert.AreSame(Other, Policy);
        }


        [PatternTestMethod(LIST_POLICIES), TestProperty(INTERFACE, nameof(IPolicies))]
        public void Get_type_miss_handler()
        {
            Assert.IsNull(Policies.Get(typeof(Type), typeof(object), OnPolicyChanged));
            Assert.IsNull(Target);
            Assert.IsNull(Type);
            Assert.IsNull(Policy);

            Policies.Set(typeof(Type), typeof(object), Instance);

            Assert.AreEqual(typeof(Type), Target);
            Assert.AreEqual(typeof(object), Type);
            Assert.AreSame(Instance, Policy);
        }


        [PatternTestMethod(LIST_POLICIES), TestProperty(INTERFACE, nameof(IPolicies))]
        public void Get_type_hit_handler()
        {
            object Other = new object();
            Policies.Set(typeof(Type), typeof(object), Instance);

            Assert.AreSame(Instance, Policies.Get(typeof(Type), typeof(object), OnPolicyChanged));
            Assert.IsNull(Target);
            Assert.IsNull(Type);
            Assert.IsNull(Policy);

            Policies.Set(typeof(Type), typeof(object), Other);

            Assert.AreEqual(typeof(Type), Target);
            Assert.AreEqual(typeof(object), Type);
            Assert.AreSame(Other, Policy);
        }



        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicies))]
        public void Get_target_type_other()
        {
            using var barrier = new Barrier(2);
            object result = null;

            Thread thread1 = new Thread(delegate ()
            {
                lock (Policies.SyncObject)
                {
                    barrier.SignalAndWait();
                    Thread.Sleep(20);
                    Policies.Set(typeof(object), typeof(Type), Instance);
                }
            })
            { Name = "1" };

            Thread thread2 = new Thread(delegate ()
            {
                barrier.SignalAndWait();
                result = Policies.Get(typeof(object), typeof(object), OnPolicyChanged);
            })
            { Name = "2" };

            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            Assert.IsNull(result);
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicies))]
        public void Get_target_type_changed()
        {
            using var barrier = new Barrier(2);
            object result = null;

            Thread thread1 = new Thread(delegate ()
            {
                lock (Policies.SyncObject)
                {
                    barrier.SignalAndWait();
                    Thread.Sleep(20);
                    Policies.Set(typeof(object), typeof(object), Instance);
                }
            })
            { Name = "1" };

            Thread thread2 = new Thread(delegate ()
            {
                barrier.SignalAndWait();
                result = Policies.Get(typeof(object), typeof(object), OnPolicyChanged);
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

        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicyList))]
        public void Clear_null_type_hit()
        {
            foreach (var type in TestTypes) Policies.Set(null, type, Instance); 
            foreach (var type in TestTypes) Policies.Clear(null, type); 
            foreach (var type in TestTypes) Assert.IsNull(Policies.Get(null, type)); 
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicyList))]
        public void Clear_null_type_miss()
        {
            // Act
            Policies.Set(null, typeof(object), Instance);
            Policies.Clear(null, typeof(Type));
            Assert.IsNotNull(Policies.Get(null, typeof(object)));
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicyList))]
        public void Clear_target_type_hit()
        {
            foreach (var type in TestTypes) Policies.Set(typeof(Type), type, Instance); 
            foreach (var type in TestTypes) Policies.Clear(typeof(Type), type); 
            foreach (var type in TestTypes) Assert.IsNull(Policies.Get(typeof(Type), type)); 
        }


        [PatternTestMethod(LIST_PATTERN), TestProperty(INTERFACE, nameof(IPolicyList))]
        public void Clear_target_type_miss()
        {
            Policies.Set(typeof(Type), typeof(object), Instance);
            Policies.Clear(typeof(object), typeof(Type));
            Assert.IsNotNull(Policies.Get(typeof(Type), typeof(object)));
        }

        #endregion
    }
}
