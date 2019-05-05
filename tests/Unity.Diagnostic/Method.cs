using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Extension;

namespace Compiled.Method
{
    [TestClass]
    public class Parameters : Unity.Specification.Diagnostic.Method.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(true).AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Validation : Unity.Specification.Diagnostic.Method.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(true).AddExtension(new ForceCompillation());
        }
    }
}

namespace Resolved.Method
{
    [TestClass]
    public class Parameters : Unity.Specification.Diagnostic.Method.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(true).AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Validation : Unity.Specification.Diagnostic.Method.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(true).AddExtension(new ForceActivation());
        }
    }
}
