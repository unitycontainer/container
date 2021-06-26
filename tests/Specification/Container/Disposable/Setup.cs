using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Container
{
    [TestClass]
    public partial class Disposal
    {
        #region Constants

        protected const string DISPOSING = "Container";
        protected const string ROOT      = "Root";
        protected const string CHILD     = "Child";

        const string SafeToDisposeMultipleTimes = "{0} safe to dispose multiple times";
        const string InstanceDisposedOnlyOnce = "Instance in {0} disposed only once";
        const string InstanceAccessibleAfterDispose = "Instance in {0} is accessible after dispose";
        const string DisposableAccessibleAfterDispose = "IDisposable in {0} is accessible after dispose";
        const string SubsequentResolutionsDisposed = "Subsequent resolutions in {0} can be disposed";
        const string IgnoresExceptionDuringDisposal = "Exceptions are ignored during disposal in {0}";
        const string DisposesWhenDiscarded = "{0} disposes when finalized";

        #endregion


        #region Fields

        protected IUnityContainer Container;

        #endregion


        #region Scaffolding

        [TestInitialize]
        public virtual void TestInitialize() => Container = new UnityContainer();

        #endregion


        #region Test Data

        public class DisposableIndicator : IDisposable
        {
            public static bool IsDisposed { get; set; }
            public void Dispose() => IsDisposed = true;
        }

        public class ExplosiveDisposable : IDisposable
        {
            #region Properties

            public int Disposals { get; protected set; }
            
            public bool IsDisposed
            {
                get => 0 < Disposals;
                private set
                {
                    if (value)
                        Disposals += 1;
                    else
                        Disposals = 0;
                }
            }

            #endregion


            #region IDisposable

            public void Dispose()
            {
                IsDisposed = true;
                throw new InvalidOperationException("Intentionally Blow Up");
            }


            #endregion
        }

        #endregion
    }
}
