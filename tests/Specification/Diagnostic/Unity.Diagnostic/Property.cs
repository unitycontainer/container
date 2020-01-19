using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled.Property
{
    [TestClass]
    public class Attribute : Unity.Specification.Property.Attribute.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic())
                                       .AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Property.Injection.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic())
                                       .AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Property.Overrides.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic())
                                       .AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Validation : Unity.Specification.Diagnostic.Property.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation())
                                       .AddExtension(new Diagnostic());
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
            return new UnityContainer().AddExtension(new Diagnostic())
                                       .AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Property.Injection.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic())
                                       .AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Property.Overrides.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic())
                                       .AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Validation : Unity.Specification.Diagnostic.Property.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation())
                                       .AddExtension(new Diagnostic());
        }
    }
}
