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
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Types : Unity.Specification.Diagnostic.Constructor.Types.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Constructor.Injection.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Attribute : Unity.Specification.Constructor.Attribute.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Constructor.Parameters.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Constructor.Overrides.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
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
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Types : Unity.Specification.Diagnostic.Constructor.Types.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Constructor.Injection.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Attribute : Unity.Specification.Constructor.Attribute.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Constructor.Parameters.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Constructor.Overrides.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }

}
