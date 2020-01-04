using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled.Property
{
    [TestClass]
    public class Attribute : Unity.Specification.Property.Attribute.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Property.Injection.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Override : Unity.Specification.Property.Overrides.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }
}


namespace Resolved.Property
{
    [TestClass]
    public class Attribute : Unity.Specification.Property.Attribute.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Property.Injection.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Override : Unity.Specification.Property.Overrides.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Activated);
        }
    }
}
