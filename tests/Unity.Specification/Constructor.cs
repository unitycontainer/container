using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled.Constructor
{
    [TestClass]
    public class Injection : Unity.Specification.Constructor.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Attribute : Unity.Specification.Constructor.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Constructor.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Constructor.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }
}


namespace Resolved.Constructor
{
    [TestClass]
    public class Injection : Unity.Specification.Constructor.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Attribute : Unity.Specification.Constructor.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Constructor.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Constructor.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }

}
