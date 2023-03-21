using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Constructor.Compiled
{
    [TestClass]
    public class Injection : Unity.Specification.Constructor.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Attribute : Unity.Specification.Constructor.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Constructor.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Constructor.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation());
        }
    }
}


namespace Constructor.Activated
{
    [TestClass]
    public class Injection : Unity.Specification.Constructor.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Attribute : Unity.Specification.Constructor.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Constructor.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Constructor.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }

}


namespace Constructor.Resolved
{
    [TestClass]
    public class Injection : Unity.Specification.Constructor.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceResolution());
        }
    }

    [TestClass]
    public class Attribute : Unity.Specification.Constructor.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceResolution());
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Constructor.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceResolution());
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Constructor.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceResolution());
        }
    }

}
