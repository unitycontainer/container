using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled.Field
{
    [TestClass]
    public class Attribute : Unity.Specification.Field.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Field.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }


    [TestClass]
    public class Overrides : Unity.Specification.Field.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }
   
}

namespace Resolved.Field
{
    [TestClass]
    public class Attribute : Unity.Specification.Field.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Field.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Field.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }
}
