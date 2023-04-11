using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Parameter.Compiled
{
    [TestClass]
    public class Attribute : Unity.Specification.Parameter.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompilation());
        }
    }

    [TestClass]
    public class Injected : Unity.Specification.Parameter.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompilation());
        }
    }

    [TestClass]
    public class Resolved : Unity.Specification.Parameter.Resolved.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompilation());
        }
    }

    [TestClass]
    public class Optional : Unity.Specification.Parameter.Optional.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompilation());
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Parameter.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompilation());
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



namespace Parameter.Resolved
{
    [TestClass]
    public class Attribute : Unity.Specification.Parameter.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceResolution());
        }
    }

    [TestClass]
    public class Injected : Unity.Specification.Parameter.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceResolution());
        }
    }

    [TestClass]
    public class Resolved : Unity.Specification.Parameter.Resolved.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceResolution());
        }
    }

    [TestClass]
    public class Optional : Unity.Specification.Parameter.Optional.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceResolution());
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Parameter.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceResolution());
        }
    }
}
