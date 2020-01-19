using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled.Method
{
    [TestClass]
    public class Attribute : Unity.Specification.Method.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Method.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Selection : Unity.Specification.Method.Selection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Method.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Method.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }
}



namespace Resolved.Method
{
    [TestClass]
    public class Attribute : Unity.Specification.Method.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Method.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Selection : Unity.Specification.Method.Selection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Method.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }

    [TestClass]
    public class Overrides : Unity.Specification.Method.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }
}
