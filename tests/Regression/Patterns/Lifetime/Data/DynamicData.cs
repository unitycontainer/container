using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity.Lifetime;
#endif

namespace Lifetime
{
    public abstract partial class Pattern : PatternBase
    {
        #region Fields

        private static Action<object, object> Action_Verify_AreNotSame = (item1, item2) => Assert.AreNotSame(item1, item2);
        private static Action<object, object> Action_Verify_AreSame = (item1, item2) => Assert.AreSame(item1, item2);
        private static Action<object, object> Action_Verify_PerResolve = (item1, item2) =>
        {
            Assert.AreNotSame(item1, item2);
            Assert.AreSame(((IView)item1).Presenter.View, item1);
            Assert.AreSame(((IView)item2).Presenter.View, item2);
            Assert.AreNotSame(item1, item2);
        };

        #endregion


        #region Test Data

        public static TestManagerSource[] Lifetime_Managers_Set = new TestManagerSource[]
        {
            #region TransientLifetimeManager

            new TestManagerSource<TransientLifetimeManager>(
                    factory: () => new TransientLifetimeManager(),
                    synchronized: false, disposable: false,
                    target: typeof(IService),
                    set_get: (item1, item2) => Assert.AreNotSame(item1, item2),

                    same_scope:            Action_Verify_AreNotSame,
                    same_scope_threads:    Action_Verify_AreNotSame,
                    child_scope:           Action_Verify_AreNotSame,
                    child_scope_threads:   Action_Verify_AreNotSame,
                    sibling_scope:         Action_Verify_AreNotSame,
                    sibling_scope_threads: Action_Verify_AreNotSame
                ),

            #endregion
            
            #region PerThreadLifetimeManager

            new TestManagerSource<PerThreadLifetimeManager>(
                    factory: () => new PerThreadLifetimeManager(),
                    synchronized: false, disposable: false,
                    target: typeof(IService),
                    set_get:                Action_Verify_AreSame,
                    same_scope:             Action_Verify_AreSame,
                    same_scope_threads:     Action_Verify_AreNotSame,
                    child_scope:            Action_Verify_AreSame,
                    child_scope_threads:    Action_Verify_AreNotSame,
                    sibling_scope:          Action_Verify_AreSame,
                    sibling_scope_threads:  Action_Verify_AreNotSame
                ),

            #endregion
            
            #region PerResolveLifetimeManager

            new TestManagerSource<PerResolveLifetimeManager>(
                    factory: () => new PerResolveLifetimeManager(),
                    synchronized: false, disposable: false,
                    target: typeof(IView),
                    set_get: (item1, item2) => Assert.AreNotSame(item1, item2),
                    same_scope:             Action_Verify_PerResolve,
                    same_scope_threads:     Action_Verify_PerResolve,
                    child_scope:            Action_Verify_PerResolve,
                    child_scope_threads:    Action_Verify_PerResolve,
                    sibling_scope:          Action_Verify_PerResolve,
                    sibling_scope_threads:  Action_Verify_PerResolve
                ),

            #endregion
            
            #region ContainerControlledTransientManager
#if !UNITY_V4
                new TestManagerSource<ContainerControlledTransientManager>(
                    factory: () => new ContainerControlledTransientManager(),
                    synchronized: false, disposable: true,
                    target: typeof(IService),
                    set_get:                Action_Verify_AreNotSame,
                    same_scope:             Action_Verify_AreNotSame,
                    same_scope_threads:     Action_Verify_AreNotSame,
                    child_scope:            Action_Verify_AreNotSame,
                    child_scope_threads:    Action_Verify_AreNotSame,
                    sibling_scope:          Action_Verify_AreNotSame,
                    sibling_scope_threads:  Action_Verify_AreNotSame
                ),
#endif
            #endregion
            
            #region ContainerControlledLifetimeManager

            new TestManagerSource<ContainerControlledLifetimeManager>(
                    factory: () => new ContainerControlledLifetimeManager(),
                    synchronized: true, disposable: true,
                    target: typeof(IService),
                    set_get:                Action_Verify_AreSame,
                    same_scope:             Action_Verify_AreSame,
                    same_scope_threads:     Action_Verify_AreSame,
                    child_scope:            Action_Verify_AreSame,
                    child_scope_threads:    Action_Verify_AreSame,
                    sibling_scope:          Action_Verify_AreSame,
                    sibling_scope_threads:  Action_Verify_AreSame
                ),

            #endregion
            
            #region HierarchicalLifetimeManager

            new TestManagerSource<HierarchicalLifetimeManager>(
                    factory: () => new HierarchicalLifetimeManager(),
                    synchronized: true, disposable: true,
                    target: typeof(IService),
                    set_get:                Action_Verify_AreSame,
                    same_scope:             Action_Verify_AreSame,
                    same_scope_threads:     Action_Verify_AreSame,
                    child_scope:            Action_Verify_AreNotSame,
                    child_scope_threads:    Action_Verify_AreNotSame,
                    sibling_scope:          Action_Verify_AreNotSame,
                    sibling_scope_threads:  Action_Verify_AreNotSame
                ),

            #endregion
            
            #region ExternallyControlledLifetimeManager

            new TestManagerSource<ExternallyControlledLifetimeManager>(
                    factory: () => new ExternallyControlledLifetimeManager(),
#if UNITY_V4
                    synchronized: false,
#else
                    synchronized: true,
#endif
                    disposable: false,
                    target: typeof(IService),
                    set_get:                Action_Verify_AreSame,
                    same_scope:             Action_Verify_AreSame,
                    same_scope_threads:     Action_Verify_AreSame,
                    child_scope:            Action_Verify_AreSame,
                    child_scope_threads:    Action_Verify_AreSame,
                    sibling_scope:          Action_Verify_AreSame,
                    sibling_scope_threads:  Action_Verify_AreSame
                )

            #endregion
        };

        #endregion
    }
}
