using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled
{

    [TestClass]
    public class Field : Unity.Specification.Diagnostic.Field.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }
}

namespace Resolved
{

    [TestClass]
    public class Field : Unity.Specification.Diagnostic.Field.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Activated);
        }
    }
}
