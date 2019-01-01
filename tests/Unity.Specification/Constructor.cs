using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled.Constructor
{
    [TestClass]
    public class Injection : Unity.Specification.Constructor.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled);
        }
    }

    [TestClass]
    public class Attribute : Unity.Specification.Constructor.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled);
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Constructor.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled);
        }
    }
}

namespace Resolved.Constructor
{
    [TestClass]
    public class Injection : Unity.Specification.Constructor.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved);
        }
    }

    [TestClass]
    public class Attribute : Unity.Specification.Constructor.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved);
        }
    }

    [TestClass]
    public class Parameters : Unity.Specification.Constructor.Parameters.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved);
        }
    }

}
