using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled.Property
{
    [TestClass]
    public class Attribute : Unity.Specification.Property.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Property.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Override : Unity.Specification.Property.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }
}


namespace Resolved.Property
{
    [TestClass]
    public class Attribute : Unity.Specification.Property.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Property.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Override : Unity.Specification.Property.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }
}
