using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled.Field
{
    [TestClass]
    public class Attribute : Unity.Specification.Field.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled);
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Field.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled);
        }
    }
}

namespace Resolved.Field
{
    [TestClass]
    public class Attribute : Unity.Specification.Field.Attribute.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved);
        }
    }

    [TestClass]
    public class Injection : Unity.Specification.Field.Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved);
        }
    }
}
