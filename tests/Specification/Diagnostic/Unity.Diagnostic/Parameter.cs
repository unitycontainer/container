using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled.Parameter
{
    [TestClass]
    public class Attribute : Unity.Specification.Parameter.Attribute.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic())
                                       .AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Injected : Unity.Specification.Parameter.Injection.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic())
                                       .AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Resolved : Unity.Specification.Parameter.Resolved.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic())
                                       .AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Optional : Unity.Specification.Parameter.Optional.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic())
                                       .AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Parameter.Overrides.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic())
                                       .AddExtension(new ForceCompillation());
        }
    }
}



namespace Resolved.Parameter
{
    [TestClass]
    public class Attribute : Unity.Specification.Parameter.Attribute.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic())
                                       .AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Injected : Unity.Specification.Parameter.Injection.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic())
                                       .AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Resolved : Unity.Specification.Parameter.Resolved.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic())
                                       .AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Optional : Unity.Specification.Parameter.Optional.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic())
                                       .AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Parameter.Overrides.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic())
                                       .AddExtension(new ForceActivation());
        }
    }
}
