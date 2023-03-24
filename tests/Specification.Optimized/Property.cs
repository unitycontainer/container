using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Property.Compiled
{
    [TestClass]
    public class Attribute : Unity.Specification.Property.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompilation());
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Property.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompilation());
        }
    }

    [TestClass]
    public class Override : Unity.Specification.Property.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompilation());
        }
    }
}


namespace Property.Activated
{
    [TestClass]
    public class Attribute : Unity.Specification.Property.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Property.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Override : Unity.Specification.Property.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }
}
