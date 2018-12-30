using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Extension;

namespace Unity.Tests.v5
{
    [TestClass]
    public class DiagnosticExtension
    {
        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        // https://github.com/unitycontainer/container/issues/122
        public void Container_122()
        {
            var container = new UnityContainer();
            container.AddNewExtension<Diagnostic>();
            
            container.RegisterType<I1, C1>();
            container.RegisterType<I2, C2>();

            //next line returns StackOverflowException
            container.Resolve<I2>();
        }

        [TestMethod]
        public void Register()
        {
            // Setup
            var container = new UnityContainer();

            // Act
            container.AddNewExtension<Diagnostic>();
            var config = container.Configure<Diagnostic>();

            // Validate
            Assert.IsNotNull(config);
        }


        [TestMethod]
        public void OptimizingResolve()
        {
            // Setup
            var container = new UnityContainer();

            // Validate
            Assert.IsNotNull(container.Resolve<object>());
        }

        [TestMethod]
        public void ForceCompile()
        {
            // Setup
            var container = new UnityContainer();

            // Act
            container.AddExtension(new Diagnostic())
                     .Configure<Diagnostic>()
                     .ForceCompile();

            // Validate
            Assert.IsNotNull(container.Resolve<object>());
        }


        [TestMethod]
        public void ForceResolving()
        {
            // Setup
            var container = new UnityContainer();

            // Act
            container.AddExtension(new Diagnostic())
                     .Configure<Diagnostic>()
                     .DisableCompile();

            // Validate
            Assert.IsNotNull(container.Resolve<object>());
        }

    }

    #region Test Data

    public interface I1 { }
    public interface I2 { }

    public class C1 : I1 { public C1(I2 i2) { } }

    public class C2 : I2 { public C2(I1 i1) { } }

    #endregion
}
