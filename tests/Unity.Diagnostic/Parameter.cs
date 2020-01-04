using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled.Parameter
{
    [TestClass]
    public class Attribute : Unity.Specification.Parameter.Attribute.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Injected : Unity.Specification.Parameter.Injection.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Resolved : Unity.Specification.Parameter.Resolved.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Optional : Unity.Specification.Parameter.Optional.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Parameter.Overrides.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
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
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Injected : Unity.Specification.Parameter.Injection.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Resolved : Unity.Specification.Parameter.Resolved.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Optional : Unity.Specification.Parameter.Optional.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Parameter.Overrides.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Activated);
        }
    }
}
