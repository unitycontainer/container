using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Method.Compiled
{
    [TestClass]
    public class Attribute : Unity.Specification.Method.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Method.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Selection : Unity.Specification.Method.Selection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Method.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Method.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation());
        }
    }
}



namespace Method.Activated
{
    [TestClass]
    public class Attribute : Unity.Specification.Method.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Method.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Selection : Unity.Specification.Method.Selection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Method.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Method.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }
}
