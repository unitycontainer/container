using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Container
{
    [TestClass]
    public partial class Basics : PatternBase
    {
        #region Scaffolding

        [TestInitialize]
        public override void TestInitialize() => base.TestInitialize();

        [ClassInitialize]
        public static void ClassInit(TestContext context) => PatternBaseInitialize(context.FullyQualifiedTestClassName);

        #endregion


        #region Test Data

        public class IUnityContainerInjectionClass
        {
            [Dependency]
            public IUnityContainer Container { get; set; }
        }

        public interface ITemporary
        {
        }

        public class Temp : ITemporary
        {
        }

        public class Temporary : ITemporary
        {
        }

        public class SpecialTemp : ITemporary //Second level
        {
        }

#pragma warning disable CA1063 // Implement IDisposable Correctly
        public class MyDisposableObject : IDisposable
        {
            private bool wasDisposed = false;

            public bool WasDisposed
            {
                get { return wasDisposed; }
                set { wasDisposed = value; }
            }

            public void Dispose()
            {
                wasDisposed = true;
            }
        }
#pragma warning restore CA1063 // Implement IDisposable Correctly

        #endregion
    }
}
