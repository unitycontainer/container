using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Method.Compiled
{
    [TestClass]
    public class Attribute : Unity.Specification.Method.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompilation());
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Method.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompilation());
        }
    }

    [TestClass]
    public class Selection : Unity.Specification.Method.Selection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompilation());
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Method.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompilation());
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Method.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompilation());
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



namespace Method.Resolved
{
    [TestClass]
    public class Attribute : Unity.Specification.Method.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceResolution());
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Method.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceResolution());
        }
    }

    [TestClass]
    public class Selection : Unity.Specification.Method.Selection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceResolution());
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Method.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceResolution());
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Method.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceResolution());
        }
    }
}
