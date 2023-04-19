using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity;
using Unity.Container.Tests;
using Unity.Lifetime;

namespace Lifetime
{
    public partial class Managers
    {
        #region Fields

        const string NAME_PATTERN = "{0} {1}";
        const string Value = "value";
        const int Hash = 11111111;

        ILifetimeContainer Scope = new CollisionScope(Hash);
        ILifetimeContainer Collision = new CollisionScope(Hash);
        LifetimeManager Manager;

        #endregion


        [PatternTestMethod(NAME_PATTERN), TestProperty(LIFETIME, nameof(HierarchicalLifetimeManager))]
        public void TryGetValue_Uninitialized()
        {
            // Arrange
            Manager = new HierarchicalLifetimeManager();

            // Validate
            Assert.AreSame(UnityContainer.NoValue, Manager.TryGetValue(Scope));
        }

        [PatternTestMethod(NAME_PATTERN), TestProperty(LIFETIME, nameof(HierarchicalLifetimeManager))]
        public void SetValue_Empty()
        {
            // Arrange
            Manager = new HierarchicalLifetimeManager();

            // Act
            Manager.SetValue(Value, Scope);

            // Validate
            Assert.AreSame(Value, Manager.TryGetValue(Scope));
        }

        [PatternTestMethod(NAME_PATTERN), TestProperty(LIFETIME, nameof(HierarchicalLifetimeManager))]
        public void SetValue_Twice()
        {
            // Arrange
            var @override = "override";
            Manager = new HierarchicalLifetimeManager();

            // Act
            Manager.SetValue(Value, Scope);
            Manager.SetValue(@override, Scope);

            // Validate
            Assert.AreSame(@override, Manager.TryGetValue(Scope));
        }

        [PatternTestMethod(NAME_PATTERN), TestProperty(LIFETIME, nameof(HierarchicalLifetimeManager))]
        public void SetValue_Collision()
        {
            // Arrange
            var @override = "override";
            Manager = new HierarchicalLifetimeManager();

            // Act
            Manager.SetValue(Value, Scope);
            Manager.SetValue(@override, Collision);

            // Validate
            Assert.AreSame(Value,     Manager.TryGetValue(Scope));
            Assert.AreSame(@override, Manager.TryGetValue(Collision));
        }

        [PatternTestMethod(NAME_PATTERN), TestProperty(LIFETIME, nameof(HierarchicalLifetimeManager))]
        public void TryGetValue_Disposed()
        {
            // Arrange
            Manager = new HierarchicalLifetimeManager();

            // Act
            var weak = SetValueWithCollectedScope(Manager);

            // Validate
            Assert.IsFalse(weak.IsAlive);
            Assert.AreSame(UnityContainer.NoValue, Manager.TryGetValue(Scope));
        }

        [PatternTestMethod(NAME_PATTERN), TestProperty(LIFETIME, nameof(HierarchicalLifetimeManager))]
        public void SetValue_Disposed()
        {
            // Arrange
            Manager = new HierarchicalLifetimeManager();

            // Act
            var weak = SetValueWithCollectedScope(Manager);
            Manager.SetValue(Value, Scope);

            // Validate
            Assert.IsFalse(weak.IsAlive);
            Assert.AreSame(Value, Manager.TryGetValue(Scope));
        }

        [PatternTestMethod(NAME_PATTERN), TestProperty(LIFETIME, nameof(HierarchicalLifetimeManager))]
        public void SetValue_Expanding()
        {
            // Arrange
            Manager = new HierarchicalLifetimeManager();
            CollisionScope disposables = null;

            // Act
            for (var i = 0; i <= 100; i++)
            {
                disposables = new CollisionScope(i);
                Manager.SetValue(i, disposables);
            }

            // Validate
            Assert.AreEqual(100, Manager.TryGetValue(disposables));
        }

        #region Implementation

        WeakReference SetValueWithCollectedScope(LifetimeManager manager)
        {
            var weak = SetValue(manager);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();
            
            return weak;

            WeakReference SetValue(LifetimeManager manager)
            {
                var scope = new CollisionScope(Hash);
                var weak = new WeakReference(scope);

                manager.SetValue(Value, scope);
                scope = new CollisionScope(Hash);

                return weak;
            }
        }

        #endregion
    }


    #region Test Data

    public class CollisionScope : List<IDisposable>, ILifetimeContainer
    {
        private int _hash;
        public CollisionScope(int hash) => _hash = hash;
        public override int GetHashCode() => _hash;

        void ILifetimeContainer.Remove(IDisposable item)
        {
            Remove(item);
        }
    }

    #endregion
}
