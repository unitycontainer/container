using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Threading;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Resolution
{
    [TestClass]
    public partial class Lazy : PatternBase
    {
        #region Scaffolding

        [TestInitialize]
        public override void TestInitialize() => base.TestInitialize();

        [ClassInitialize]
        public static void ClassInit(TestContext context) => PatternBaseInitialize(context.FullyQualifiedTestClassName);

        #endregion


        #region Test Data


        public interface IBase
        {
            IService Service { get; set; }
        }

        public interface ILazyDependency
        {
            Lazy<EmailService> Service { get; set; }
        }

        public class Base : IBase
        {
            [Dependency]
            public IService Service { get; set; }
        }

        public class LazyDependency : ILazyDependency
        {
            [Dependency]
            public Lazy<EmailService> Service { get; set; }
        }

        public class LazyDependencyConstructor
        {
            private Lazy<EmailService> service;

            public LazyDependencyConstructor(Lazy<EmailService> s)
            {
                service = s;
            }
        }

        public class EmailService : IService, IDisposable
        {
            public string Id { get; } = Guid.NewGuid().ToString();

            public bool Disposed;
            public void Dispose()
            {
                Disposed = true;
            }
        }

        public class ObjectThatGetsALazy
        {
            [Dependency]
            public Lazy<IService> LoggerLazy { get; set; }
        }

        public class ObjectThatGetsMultipleLazy
        {
            [Dependency]
            public Lazy<IService> LoggerLazy1 { get; set; }

            [Dependency]
            public Lazy<IService> LoggerLazy2 { get; set; }
        }

        #endregion
    }
}
