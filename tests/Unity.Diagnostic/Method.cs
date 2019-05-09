using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled.Method
{
    [TestClass]
    public class Parameters : Unity.Specification.Diagnostic.Method.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Validation : Unity.Specification.Diagnostic.Method.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
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
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Validation : Unity.Specification.Diagnostic.Method.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Activated);
        }
    }
}
