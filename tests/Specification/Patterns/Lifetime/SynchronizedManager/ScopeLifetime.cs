using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity.Lifetime;
#endif

namespace Lifetime.Synchronization
{
    public abstract partial class Pattern
    {
#if !UNITY_V4
        [PatternTestMethod("SetValue(object) is not added to the scope"), TestCategory(LIFETIME_MANAGEMENT)]
        [DynamicData(nameof(Synchronized_Managers_Data), typeof(Lifetime.Pattern))]
        public virtual void SetValueObjectIsNotAddedToScope(LifetimeManager manager)
        {
            var instance = new object();
            var scope = new LifetimeContainer();

            manager.SetTestValue(instance, scope);

            Assert.AreSame(instance, manager.GetTestValue(scope));
            Assert.IsFalse(scope.Contains(instance));
        }

        [PatternTestMethod("SetValue(IDisposable) adds to the scope"), TestCategory(LIFETIME_MANAGEMENT)]
        [DynamicData(nameof(Synchronized_Managers_Data), typeof(Lifetime.Pattern))]
        public virtual void SetValueDisposableAddsToScope(LifetimeManager manager)
        {
            if (manager is ExternallyControlledLifetimeManager) return;

            object instance = new TestDisposable();
            var scope = new LifetimeContainer();

            manager.SetTestValue(instance, scope);

            Assert.AreSame(instance, manager.GetTestValue(scope));
            Assert.IsTrue(scope.Contains(instance));
        }
#endif
    }
}
