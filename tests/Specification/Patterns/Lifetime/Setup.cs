using Regression;
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Lifetime;
#endif

namespace Lifetime
{
    public abstract partial class Pattern : PatternBase
    {
        #region Constants

        protected const string SAME_SCOPE  = "Same Scope";
        protected const string CHILD_SCOPE = "Child Scope";
        protected const string SIBLING_SCOPES  = "Sibling Scopes";
        protected const string LIFETIME_MANAGER = "Manager";
        protected const string SYNCHRONIZED_MANAGER = "Synchronization";
        protected const string LIFETIME_MANAGEMENT = "Scope";

        #endregion


        #region Delegates

#if UNITY_V4
        public delegate LifetimeManager LifetimeManagerFactoryDelegate();
#else
        public delegate ITypeLifetimeManager LifetimeManagerFactoryDelegate();
#endif

        #endregion


        #region Fields

        protected object Item1;
        protected object Item2;
        protected Type TargetType;

        #endregion


        #region Implementation

        protected virtual bool ArrangeTest(LifetimeManagerFactoryDelegate factory, Type type, IUnityContainer child = null)
        {
            var manager = factory();
#if UNITY_V4
            Container.RegisterType(typeof(IService), typeof(Service), manager)
                     .RegisterType(typeof(Service), factory())
                     .RegisterType<IPresenter, MockPresenter>()
                     .RegisterType<IView, View>(factory());
#else
            Container.RegisterType(typeof(IService), typeof(Service), (ITypeLifetimeManager)manager)
                     .RegisterType(typeof(Service), (ITypeLifetimeManager)factory())
                     .RegisterType<IPresenter, MockPresenter>()
                     .RegisterType<IView, View>((ITypeLifetimeManager)factory());
#endif
            TargetType = type;

            if (child is not null)
            {
#if UNITY_V4
                child.RegisterType(typeof(IService), typeof(Service), factory())
                     .RegisterType(typeof(Service), factory())
                     .RegisterType<IPresenter, MockPresenter>()
                     .RegisterType<IView, View>(factory());
#else
                child.RegisterType(typeof(IService), typeof(Service), (ITypeLifetimeManager)factory())
                     .RegisterType(typeof(Service), (ITypeLifetimeManager)factory())
                     .RegisterType<IPresenter, MockPresenter>()
                     .RegisterType<IView, View>((ITypeLifetimeManager)factory());
#endif
            }

            return manager is PerResolveLifetimeManager
                ? true : false;
        }

        #endregion


        #region Test Types

        public interface IPresenter
        {
            IView View { get; }
        }

        public class MockPresenter : IPresenter
        {
            public IView View { get; set; }

            public MockPresenter(IView view)
            {
                View = view;
            }
        }

        public interface IView
        {
            IPresenter Presenter { get; set; }
        }

        public class View : IView
        {
            [Dependency]
            public IPresenter Presenter { get; set; }
        }

        protected class TestDisposable : IDisposable
        {
            int _count;

            // Dispose only once
            public TestDisposable()
            {
                _count = 1;
            }

            // Allows multiple disposes
            public TestDisposable(int count)
            {
                _count = count;
            }

            public bool IsDisposed => 0 >= _count;

            public void Dispose()
            {
                if (IsDisposed) throw new InvalidOperationException("Disposed already");
                
                _count -= 1;
            }
        }

        #endregion


        #region Test Data

        public static IEnumerable<object[]> Lifetime_Managers_Data
            => Lifetime_Managers_Set.Select(source => new object[] { source.Factory() });


        public static IEnumerable<object[]> Synchronized_Managers_Data
            => Lifetime_Managers_Set.Where(source => source.IsSynchronized)
                                    .Select(source => new object[] { source.Factory() });

        #endregion
    }

    public static class PatternExtensions
    {
        public static object GetTestValue(this LifetimeManager manager, object scope)
        {
#if UNITY_V4
            return manager.GetValue();
#elif UNITY_V5
            return manager.GetValue((ILifetimeContainer)scope);
#else
            return manager.GetValue((ICollection<IDisposable>)scope);
#endif

        }

#if UNITY_V4
        public static object TryGetValue(this LifetimeManager manager, object scope)
        {
            return manager.GetValue();
        }
#endif

        public static void SetTestValue(this LifetimeManager manager, object value, object scope)
        {
#if UNITY_V4
            manager.SetValue(value);
#elif UNITY_V5
            manager.SetValue(value, (ILifetimeContainer)scope);
#else
            manager.SetValue(value, (ICollection<IDisposable>)scope);
#endif
        }

#if !UNITY_V4 && !UNITY_V5
#endif

    }
}
