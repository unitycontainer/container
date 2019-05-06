using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Extension;

namespace Compiled.Constructor
{
    [TestClass]
    public class Annotation : Unity.Specification.Diagnostic.Constructor.Annotation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(true).AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Diagnostic.Constructor.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(true).AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Types : Unity.Specification.Diagnostic.Constructor.Types.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(true).AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Diagnostic.Constructor.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(true).AddExtension(new ForceCompillation());
        }
    }
}

namespace Resolved.Constructor
{
    [TestClass]
    public class Annotation : Unity.Specification.Diagnostic.Constructor.Annotation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(true).AddExtension(new ForceCompillation());
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Diagnostic.Constructor.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(true).AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Types : Unity.Specification.Diagnostic.Constructor.Types.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(true).AddExtension(new ForceActivation());
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Diagnostic.Constructor.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(true).AddExtension(new ForceActivation());
        }
    }
}
