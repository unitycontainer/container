using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Parameter.Compiled
{
    [TestClass]
    public class Attribute : Unity.Specification.Parameter.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Injected : Unity.Specification.Parameter.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Resolved : Unity.Specification.Parameter.Resolved.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Optional : Unity.Specification.Parameter.Optional.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Parameter.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation());
        }
    }
}



namespace Parameter.Activated
{
    [TestClass]
    public class Attribute : Unity.Specification.Parameter.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Injected : Unity.Specification.Parameter.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Resolved : Unity.Specification.Parameter.Resolved.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Optional : Unity.Specification.Parameter.Optional.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Parameter.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }
}
